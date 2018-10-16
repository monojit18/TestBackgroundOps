using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TestBackgroundOps
{
    public class TestBkgdDownloadTask
    {
        private HttpClient _httpClient;
        private SemaphoreSlim _poolSemaphore;
        private SemaphoreSlim _chunkSemaphore;
        private CancellationTokenSource _tokenSource;
        private int _chunkIndex;

        private async Task<byte[]> DownloadImageAsync(string segmentString)
        {

            _httpClient = new HttpClient();
            _tokenSource = new CancellationTokenSource();
            _httpClient.BaseAddress = new Uri("https://placeholder.com/");

            var httpResponse = await _httpClient.GetAsync(segmentString, _tokenSource.Token);
            var imageBytes = await httpResponse.Content.ReadAsByteArrayAsync();
            Console.WriteLine($"{segmentString}: " + imageBytes.Length.ToString());
            return imageBytes;

        }

        public List<string> PrepareURLsList()
        {

            var urlsList = new List<string>();
            for (int i = _chunkIndex, j = 10; i < _chunkIndex + 2000;++i, ++j)
            {

                var urlString = $"https://placeholder.com/{i}x{j}";
                urlsList.Add(urlString);

            }

            return urlsList;                

        }

        public string PrepareCurrentURL()
        {

            int i = _chunkIndex, j = 10;
            var urlString = $"https://placeholder.com/{i}x{j}";
            return urlString;

        }

        public async Task PoolImagesAsync()
        {

            var imagesList = new List<string>();
            for (int i = 10, j = 10; i < 5000; ++i, ++j)
                imagesList.Add($"{i}x{j}");

            await Task.Run(async () =>
            {

                var tasksArray = imagesList.Select(async (string val) =>
                {

                    await _poolSemaphore.WaitAsync();
                    await DownloadImageAsync(val);
                    _poolSemaphore.Release();

                }).ToArray(); ;

                await Task.WhenAll(tasksArray);

            });


            Console.WriteLine(imagesList.Count.ToString());

        }

        public async Task ChunkDownloadImagesAsync()
        {

            var imagesList = new List<string>();
            for (int i = _chunkIndex, j = 10; i < _chunkIndex + 200; ++i, ++j)
                imagesList.Add($"{i}x{j}");

            await Task.Run(async () =>
            {

                var tasksArray = imagesList.Select(async (string val) =>
                {

                    await _chunkSemaphore.WaitAsync();
                    await DownloadImageAsync(val);
                    _chunkSemaphore.Release();

                }).ToArray(); ;

                await Task.WhenAll(tasksArray);

            });


            Console.WriteLine(imagesList.Count.ToString());

        }

        public async Task<int> SingleDownloadImageAsync()
        {

            var val = $"{_chunkIndex}x{_chunkIndex}";
            var imageBytes = await DownloadImageAsync(val);
            ++_chunkIndex;
            return imageBytes.Length;

        }

        public TestBkgdDownloadTask()
        {

            _poolSemaphore = new SemaphoreSlim(5);
            _chunkSemaphore = new SemaphoreSlim(3);
            _chunkIndex = 10;

        }

        public void Cancel()
        {

            _tokenSource.Cancel();

        }

    }
}
