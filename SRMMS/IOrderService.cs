using SRMMS.DTOs;

namespace SRMMS
{
    public interface IOrderService
    {
        Task<int> CreateOrder(OrderDTO orderDto);
    }
}