//namespace CloudAndCO.Service
//{
//    public class ApiClient
//    {
//        private readonly HttpClient _http;
//        public ApiClient(HttpClient http) => _http = http;

//        //___ BLOBS ___
//        public async Task<List<BlobFile>?> ListBlobsAsync(string container, string? prefix = null)
//     => await _http.GetFromJsonAsync<List<BlobFile>>(
//         $"blobs/{container}{(string.IsNullOrEmpty(prefix) ? "" : $"?prefix={Uri.EscapeDataString(prefix)}")}");

//        // In your ApiClient.cs, ensure the URL is correct
//        //public async Task<List<BlobFile>> ListBlobsAsync(string container, string prefix)
//        //{
//        //    // Debug: Check what URL you're calling
//        //    var url = $"api/blobs/{container}?prefix={prefix}";
//        //    Console.WriteLine($"Calling: {url}"); // Add logging

//        //    return await _http.GetFromJsonAsync<List<BlobFile>>(url);
//        //}

//        public async Task<Stream> GetBlobAsync(string container, string name)
//            => await _http.GetStreamAsync($"blobs/{container}/{Uri.EscapeDataString(name)}");

//        public async Task<bool> UploadBlobAsync(string container, string name, Stream content, string? contentType = null)
//        {
//            using var c = new StreamContent(content);
//            if (!string.IsNullOrEmpty(contentType))
//                c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
//            var res = await _http.PostAsync($"blobs/{container}/{Uri.EscapeDataString(name)}", c);
//            return res.IsSuccessStatusCode;
//        }

//        public async Task<bool> DeleteBlobAsync(string container, string name)
//            => (await _http.DeleteAsync($"blobs/{container}/{Uri.EscapeDataString(name)}")).IsSuccessStatusCode;

//        // ___ TABLES ___
//        public record UpsertEntityDto(string PartitionKey, string RowKey, Dictionary<string, object>? Properties);

//        public async Task<bool> UpsertEntityAsync(string table, UpsertEntityDto dto)
//            => (await _http.PostAsJsonAsync($"tables/{table}", dto)).IsSuccessStatusCode;

//        public async Task<dynamic?> GetEntityAsync(string table, string pk, string rk)
//            => await _http.GetFromJsonAsync<dynamic>($"tables/{table}/bykey/{Uri.EscapeDataString(pk)}/{Uri.EscapeDataString(rk)}");

//        public async Task<dynamic?> QueryPartitionAsync(string table, string pk)
//            => await _http.GetFromJsonAsync<dynamic>($"tables/{table}/partition/{Uri.EscapeDataString(pk)}");

//        public async Task<bool> DeleteEntityAsync(string table, string pk, string rk)
//            => (await _http.DeleteAsync($"tables/{table}/bykey/{Uri.EscapeDataString(pk)}/{Uri.EscapeDataString(rk)}")).IsSuccessStatusCode;

//        // ___ QUEUES ___   

//        public async Task<bool> SendMessageAsync(string queue, string message)
//            => (await _http.PostAsJsonAsync($"queues/{queue}", new { message })).IsSuccessStatusCode;

//        public record QueueMessage(string MessageId, string PopReceipt, string MessageText, DateTimeOffset InsertionTime, DateTimeOffset ExpirationTime, DateTimeOffset NextVisibleTime);

//        public async Task<QueueMessage?> ReceiveMessageAsync(string queue)
//            => await _http.GetFromJsonAsync<QueueMessage?>($"queues/{queue}");

//        public async Task<List<QueueMessage>?> PeekMessagesAsync(string queue, int maxMessages = 32)
//            => await _http.GetFromJsonAsync<List<QueueMessage>?>($"queues/{queue}/peek?maxMessages={maxMessages}");

//        public async Task<bool> DeleteMessageAsync(string queue, string messageId, string popReceipt)
//            => (await _http.DeleteAsync($"queues/{queue}/{Uri.EscapeDataString(messageId)}/{Uri.EscapeDataString(popReceipt)}")).IsSuccessStatusCode;


//        // ___ CUSTOMER ___  
//        public async Task<List<Customer>?> GetAllCustomersAsync()
//            => await _http.GetFromJsonAsync<List<Customer>>("customers");

//        public async Task<Customer?> GetCustomerByIdAsync(string id)
//            => await _http.GetFromJsonAsync<Customer>($"customers/{id}");

//        public async Task<bool> CreateCustomerAsync(Customer customer)
//            => (await _http.PostAsJsonAsync("customers", customer)).IsSuccessStatusCode;
//    }
//}
