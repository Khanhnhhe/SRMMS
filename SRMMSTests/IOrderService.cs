using SRMMS.DTOs;

namespace SRMMSTests
{
    public interface IOrderService
    {
        Task<int> CreateOrder(OrderDTO orderDto);
    }
}