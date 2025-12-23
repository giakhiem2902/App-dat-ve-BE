using AppdatveCore.Entities;
using AppdatveInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppdatveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(AppDbContext context) : ControllerBase
{
    
    /// Lấy danh sách tất cả các hãng xe (Mọi người đều có quyền xem)
    /// Flutter sẽ gọi API này để hiển thị danh sách hãng ở màn hình Home.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusCompany>>> GetCompanies()
    {
        return await context.BusCompanies
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// Lấy thông tin chi tiết 1 hãng xe kèm danh sách xe của hãng đó
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BusCompany>> GetCompany(int id)
    {
        var company = await context.BusCompanies
            .Include(c => c.Id)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null) return NotFound("Không tìm thấy hãng xe.");

        return company;
    }

    /// <summary>
    /// Thêm hãng xe mới (Chỉ Admin mới có quyền thực hiện)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BusCompany>> CreateCompany(BusCompany company)
    {
        // Kiểm tra xem tên hãng đã tồn tại chưa
        if (await context.BusCompanies.AnyAsync(c => c.Name == company.Name))
            return BadRequest("Tên hãng xe này đã tồn tại.");

        context.BusCompanies.Add(company);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
    }

    /// Cập nhật thông tin hãng xe (Chỉ Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCompany(int id, BusCompany company)
    {
        if (id != company.Id) return BadRequest();

        context.Entry(company).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!context.BusCompanies.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    /// Xóa hãng xe (Chỉ Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var company = await context.BusCompanies.FindAsync(id);
        if (company == null) return NotFound();

        // Kiểm tra xem hãng có đang sở hữu xe nào không trước khi xóa
        var hasBuses = await context.Busess.AnyAsync(b => b.BusCompanyId == id);
        if (hasBuses) return BadRequest("Không thể xóa hãng xe đang có xe hoạt động.");

        context.BusCompanies.Remove(company);
        await context.SaveChangesAsync();

        return NoContent();
    }
}