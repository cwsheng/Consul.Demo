using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consul.Client.Services
{
    public class CallService : ICallService
    {
        private readonly IConfiguration _configuration;
        private readonly ConsulClient _consulClient;

        private ConcurrentBag<string> _serviceAUrls;
        private ConcurrentBag<string> _serviceBUrls;

        private IHttpClientFactory _httpClient;

        public CallService(IConfiguration configuration, IHttpClientFactory httpClient)
        {
            _configuration = configuration;

            _consulClient = new ConsulClient(options =>
            {
                options.Address = new Uri(_configuration["Consul:Address"]);
            });

            _httpClient = httpClient;
        }

        public async Task<string> GetGoodsService()
        {
            if (_serviceAUrls == null)
                return await Task.FromResult("Goods Service Initializing...");

            using var httpClient = _httpClient.CreateClient();

            //随机获取一个服务地址调用
            var serviceUrl = _serviceAUrls.ElementAt(new Random().Next(_serviceAUrls.Count()));

            Console.WriteLine("Goods Service：" + serviceUrl);

            var result = await httpClient.GetStringAsync($"{serviceUrl}/goods");

            return result;
        }

        public async Task<string> GetOrderService()
        {
            if (_serviceBUrls == null)
                return await Task.FromResult("Order Service Initializing...");

            using var httpClient = _httpClient.CreateClient();

            //随机获取一个服务地址调用
            var serviceUrl = _serviceBUrls.ElementAt(new Random().Next(_serviceBUrls.Count()));

            Console.WriteLine("Order Service：" + serviceUrl);

            var result = await httpClient.GetStringAsync($"{serviceUrl}/order");

            return result;
        }

        public void InitServiceList()
        {
            var serviceNames = new string[] { "czapigoods", "czapiorder" };

            foreach (var item in serviceNames)
            {
                Task.Run(async () =>
                {
                    var queryOptions = new QueryOptions
                    {
                        WaitTime = TimeSpan.FromMinutes(5)
                    };
                    while (true)
                    {
                        await InitServicesAsync(queryOptions, item);
                    }
                });
            }
        }
        private async Task InitServicesAsync(QueryOptions queryOptions, string serviceName)
        {
            //获取心跳检查服务
            var result = await _consulClient.Health.Service(serviceName, null, true, queryOptions);

            if (queryOptions.WaitIndex != result.LastIndex)
            {
                queryOptions.WaitIndex = result.LastIndex;

                var services = result.Response.Select(x => $"http://{x.Service.Address}:{x.Service.Port}");

                if (serviceName == "czapigoods")
                {
                    _serviceAUrls = new ConcurrentBag<string>(services);
                }
                else if (serviceName == "czapiorder")
                {
                    _serviceBUrls = new ConcurrentBag<string>(services);
                }
            }
        }
    }
}
