using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestBackgroundOps
{
    public class TestBkgdUploadTask
    {
    
        private SemaphoreSlim _poolSemaphore;

        private async Task<string> UploadPostsAsync(Item uploadItem)
        {

            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");

                var uploadString = JsonConvert.SerializeObject(uploadItem);
                var content = new StringContent(uploadString, Encoding.UTF8, "application/json");

                var httpResponse = await httpClient.PostAsync("posts", content);
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                return responseString;

            }


        }

        public TestBkgdUploadTask()
        {
            _poolSemaphore = new SemaphoreSlim(5);
        }

        public async Task UploadPostsAsync()
        {

            var postsList = new List<Item>();
            for (int i = 0; i < 5000; ++i)
            {

                var uploadItem = new Item()
                {

                    ItemId = i.ToString(),
                    Text = $"Test{i}",
                    Description = $"Desc{i}"

                };

                postsList.Add(uploadItem);

            }

            await Task.Run(async () =>
            {

                var tasksArray = postsList.Select(async (Item uploadItem) =>
                {

                    await _poolSemaphore.WaitAsync();
                    var responseString = await UploadPostsAsync(uploadItem);
                    Console.WriteLine(responseString);
                    _poolSemaphore.Release();

                }).ToArray(); ;

                await Task.WhenAll(tasksArray);

            });


            Console.WriteLine(postsList.Count.ToString());

        }
    }
}
