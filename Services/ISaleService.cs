using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public interface ISaleService
{
    Task<IEnumerable<SaleDto>> GetAllSalesAsync();
    Task<SaleDto?> GetSaleByIdAsync(int id);
    Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto);
    Task<SaleDto?> UpdateSaleAsync(int id, UpdateSaleDto updateSaleDto);
    Task<bool> DeleteSaleAsync(int id);
    Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SaleDto>> GetSalesByEmployeeAsync(int employeeId);
    Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId);
}

