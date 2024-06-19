using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using System.Net.Http;
using Newtonsoft.Json;
using ParcelTrackerTelegramBot.Models;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using Telegram.Bots.Http;
using Telegram.Bots;
using ParcelTrackerTelegramBot.Models.TrackingDetailsModel;


namespace ParcelTrackerTelegramBot
{
    public class ParcelTrackerBot
    {
        TelegramBotClient client = new TelegramBotClient("7027427404:AAGKSfToYvly4uEVXYprG1kMA5_fXCGFhiY");
        CancellationToken cancellationToken = new CancellationToken();

        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

        public async Task BotStart()
        {
            client.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cancellationToken);
            var botMe = await client.GetMeAsync();
        }

        public struct ParcelData
        {
            public string code;
            public string tag;
            public string description;
        }

        int input_mode = 0;
        public ParcelData parcel;

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            bool text_flag = update.Type == UpdateType.Message && update?.Message?.Text != null;
            bool call_flag = update.Type == UpdateType.CallbackQuery;

            var message = update.Message;

            if (text_flag && message.Text == "/start")
            {
                await StartMessage();
            }
            else if (text_flag && message.Text == "Моя квота")
            {
                await MyQuota();
            }
            else if (text_flag && message.Text == "Мій список")
            {
                await MyList();
            }
            else if (text_flag && message.Text == "Додати в мій список")
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Введіть номер відправлення:");
                input_mode = 1;
            }
            else if (text_flag && message.Text == "Відслідкувати")
            {
                await ItemsList();
                input_mode = 4;
            }
            else if (text_flag && message.Text == "Видалити з мого списку")
            {
                await ItemsList();
                input_mode = 5;
            }


            // ---  Алгоритм додавання посилки в список  ---
            else if (text_flag && (input_mode == 1 && input_mode == 2 && input_mode == 3 || message.Type == MessageType.Text))
            {
                if (message.Text != "" && input_mode == 1)
                {
                    parcel.code = message.Text;
                    await client.SendTextMessageAsync(message.Chat.Id, "Введіть тег відправлення:");
                    input_mode++;
                }
                else if (message.Text != "" && input_mode == 2)
                {
                    parcel.tag = message.Text;
                    await client.SendTextMessageAsync(message.Chat.Id, "Введіть опис відправлення:");
                    input_mode++;

                }
                else if (message.Text != "" && input_mode == 3)
                {
                    parcel.description = message.Text;

                    await AddToMyList(parcel);

                    input_mode = 0;
                    parcel.code = "";
                    parcel.tag = "";
                    parcel.description = "";
                }
            }
            // ---------------------------------------------

            //  Відслідковування посилки зі списку
            else if (call_flag && input_mode == 4)
            {
                string? code = update.CallbackQuery.Data;
                await Track(code);
            }

            //  Видалення посилки зі списку
            else if (call_flag && input_mode == 5)
            {
                string? code = update.CallbackQuery.Data;
                await RemoveFromMyList(code);  
            }


            else
            {
                try 
                {
                    if (text_flag)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Нерозпізнана команда !");
                    }
                    else if (call_flag)
                    { 
                        await client.SendTextMessageAsync(update.CallbackQuery.From.Id, "Нерозпізнана команда !");
                    }
                }
                catch (System.NullReferenceException)
                {
                    Console.WriteLine("UNHANDLED MESSAGE ERROR !");
                }
            }


            async Task StartMessage()
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Вітаю в ParcelTrackingBot !\nВідслідковуй свої міжнародні відправлення просто ввівши номер-ідентифікатор !");

                ReplyKeyboardMarkup replyKeyboardMarkup = new
                (new[]
                    {
                        new KeyboardButton[] { "Мій список", "Моя квота" },
                        new KeyboardButton[] { "Додати в мій список", "Видалити з мого списку" },
                        new KeyboardButton[] { "Відслідкувати" }
                    }
                )
                {
                    ResizeKeyboard = true
                };

                await client.SendTextMessageAsync(message.Chat.Id, "Для роботи оберіть пункт:", replyMarkup: replyKeyboardMarkup);
            }

            async Task MyQuota()
            {
                string path = "http://localhost:35957/QuotaInfo/GetModel";
                var http_client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(path)
                };

                try 
                {
                    var response = await http_client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<QuotaInfoModel>(body);

                    await client.SendTextMessageAsync(message.Chat.Id, $"Ваша квота:\n{result.data.quota_remain} з {result.data.quota_total}");
                }
                catch (Exception e) { 
                    Console.WriteLine("(MyQuota) ERROR: " + e.Message);
                }

                return;
            }

            async Task MyList()
            {
                string url = "http://localhost:35957/MyParcelsList?user_id=" + message.From.Id;
                var http_client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                try
                {
                    var response = await http_client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<DatabaseModel>>(body);
                    
                    string output_message = "";

                    if (result.Count() == 0)
                    {
                        output_message = "Ваш список пустий !\nСпершу додайте елементи, обравши \"Додати в мій список\"";
                    }
                    else
                    {
                        int counter = 1;
                        foreach (DatabaseModel item in result)
                        {
                            output_message += $"{counter}. {item.Parcel_code}\nTag: {item.Parcel_tag}\nDescription: {item.Parcel_description}\nReg. time: {item.Registration_time} UTC\n\n";
                            counter++;
                        }
                    }

                    await client.SendTextMessageAsync(message.Chat.Id, output_message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("(MyList) ERROR: " + e.Message);
                }

                return;
            }

            async Task AddToMyList(ParcelData parcel)
            {

                string url = $"http://localhost:35957/ParcelRegister?user_id={message.From.Id}&parcel_code={parcel.code}&parcel_tag={parcel.tag}&parcel_description={parcel.description}";

                try
                {
                    var http_client = new HttpClient();
                    var response = await http_client.PostAsync(url, null);
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();

                    if (body == "200")
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Відправлення успішно додано до списку !");
                        return;
                    }
                    else
                    {
                        string error = "";
                        switch (body)
                        {
                            case "-18019903":
                                error = "Номер не розпізнано системою";
                                break;
                            case "-18019901":
                                error = "Номер уже зареєстрований в системі";
                                break;
                            case "-18010012":
                                error = "Номер не розпізнано системою";
                                break;
                            default:
                                error = body;
                                break;
                        }

                        await client.SendTextMessageAsync(message.Chat.Id, "Помилка на стороні сервера: " + error);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("(AddToMyList) ERROR: " + e.Message);
                }

            }

            async Task Track(string parcel_code)
            {
                string url = $"http://localhost:35957/TrackingDetails?ParcelCode={parcel_code}";

                var http_client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                try
                { 
                    var response = await http_client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TrackingDetailsModel>(body);

                    var latest_status = result.data.accepted.First().track_info.latest_status;
                    var latest_event = result.data.accepted.First().track_info.latest_event;

                    string output = 
                        "--- Деталі доставки ---\n" +
                        $"Cтатус: {latest_status.status}\n" +
                        $"Остання подія: \"{latest_event.description}\" о {latest_event.time_iso}\n" +
                        $"Суб-статус {latest_event.sub_status}\n" +
                        $"Локація: {latest_event.location}\n" +
                        $"Стадія: {latest_event.stage}\n" +
                        $"Адреса: \n" +
                        $"Країна - {latest_event.address.country}\n" +
                        $"Регіон - {latest_event.address.state}\n" +
                        $"Місто - {latest_event.address.city}\n" +
                        $"Вулиця - {latest_event.address.street}\n" +
                        $"Поштовий індекс - {latest_event.address.postal_code}\n" +
                        $"Координати - {latest_event.address.coordinates.longitude} {latest_event.address.coordinates.latitude}\n" +
                        $"\n" +
                        $"";

                    await client.SendTextMessageAsync(update.CallbackQuery.From.Id, output);
                }
                catch (Exception e)
                {
                    Console.WriteLine("(Track) ERROR: " + e.Message);
                }
                
            }

            async Task RemoveFromMyList(string parcel_code)
            {
                string url = $"http://localhost:35957/ParcelDelete?parcel_code={parcel_code}";

                var http_client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url)
                };

                try
                { 
                    var response = await http_client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();

                    if (body == "200")
                    {
                        await client.SendTextMessageAsync(update.CallbackQuery.From.Id, "Елемент успішно видалено зі списку !");
                        return;
                    }
                    else
                    {
                        string error = "";
                        switch (body)
                        {
                            case "-18019903":
                                error = "Номер не розпізнано системою";
                                break;
                            case "-18019901":
                                error = "Номер уже зареєстрований в системі";
                                break;
                            case "-18010012":
                                error = "Номер не розпізнано системою";
                                break;
                            default:
                                error = body;
                                break;
                        }

                        await client.SendTextMessageAsync(update.CallbackQuery.From.Id, "Помилка на стороні сервера: " + error);
                    }
                    
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("(RemoveFromMyList) ERROR: " + e.Message);
                }
                
            }

            async Task ItemsList()
            {
                string url = "http://localhost:35957/MyParcelsList?user_id=" + message.From.Id;

                var http_client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                try
                { 
                    var response = await http_client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<DatabaseModel>>(body);

                    if (result.Count() == 0)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваш список пустий !\nСпершу додайте елементи, обравши \"Додати в мій список\"");
                        return;
                    }
                    else
                    {
                        List<InlineKeyboardButton> list = new List<InlineKeyboardButton>();

                        foreach (var s in result)
                        {
                            list.Add(InlineKeyboardButton.WithCallbackData(s.Parcel_code));
                        }
                        var inline = new InlineKeyboardMarkup(list);
                        await client.SendTextMessageAsync(message.Chat.Id, "Оберіть відправлення", replyMarkup: inline);

                        return;  
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("(ItemsList) ERROR: " + e.Message);
                }


            }

        }
    
    

        private Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"TelegramBOT error: {apiRequestException.Message}",
                _ => ToString()
            };
            Console.WriteLine("ERROR: " + ErrorMessage);
            return Task.CompletedTask;
        }

    }
}
