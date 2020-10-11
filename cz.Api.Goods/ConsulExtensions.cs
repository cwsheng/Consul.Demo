using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cz.Api.Goods
{
    public static class ConsulExtensions
    {
        public static IApplicationBuilder UseConul(this IApplicationBuilder app, IConfiguration configuration, IHostApplicationLifetime lifetime)
        {
            var client = new ConsulClient(options =>
            {
                // Consul客户端地址
                options.Address = new Uri(configuration["Consul:Address"]);
            });

            var registration = new AgentServiceRegistration
            {
                // 唯一Id
                ID = Guid.NewGuid().ToString(),
                // 服务名
                Name = configuration["Consul:Name"],
                // 服务绑定IP
                Address = configuration["Consul:Ip"],
                // 服务绑定端口
                Port = Convert.ToInt32(configuration["Consul:Port"]),
                Check = new AgentServiceCheck
                {
                    // 服务启动多久后注册
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查时间间隔
                    Interval = TimeSpan.FromSeconds(10),
                    // 健康检查地址
                    HTTP = $"http://{configuration["Consul:Ip"]}:{configuration["Consul:Port"]}{configuration["Consul:HealthCheck"]}",
                    // 超时时间
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };

            // 注册服务
            client.Agent.ServiceRegister(registration).Wait();

            // 应用程序终止时，取消服务注册
            lifetime.ApplicationStopping.Register(() =>
            {
                client.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
