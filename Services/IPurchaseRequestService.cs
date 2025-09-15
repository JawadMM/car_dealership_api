using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public interface IPurchaseRequestService
{
    Task<IEnumerable<PurchaseRequestDto>> GetAllPurchaseRequestsAsync();
    Task<IEnumerable<PurchaseRequestDto>> GetPurchaseRequestsByCustomerAsync(string customerId);
    Task<PurchaseRequestDto?> GetPurchaseRequestByIdAsync(int id);
    Task<PurchaseRequestDto> CreatePurchaseRequestAsync(CreatePurchaseRequestDto createDto, string customerId);
    Task<PurchaseRequestDto?> UpdatePurchaseRequestAsync(int id, UpdatePurchaseRequestDto updateDto);
    Task<bool> DeletePurchaseRequestAsync(int id);
    Task<IEnumerable<PurchaseRequestDto>> GetPendingPurchaseRequestsAsync();
}

