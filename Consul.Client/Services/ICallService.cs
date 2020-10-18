using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consul.Client.Services
{
    public interface ICallService
    {
        /// <summary>
        /// 获取 Goods Service 返回数据
        /// </summary>
        /// <returns></returns>
        Task<string> GetGoodsService();

        /// <summary>
        /// 获取 Order Service 返回数据
        /// </summary>
        /// <returns></returns>
        Task<string> GetOrderService();

        /// <summary>
        /// 初始化服务
        /// </summary>
        void InitServiceList();
    }
}
