using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_ADONET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeProductController : ControllerBase
    {
        private readonly ITypeProductRepository _typeProductRepository;

        public TypeProductController(ITypeProductRepository typeProductRepository)
        {
            _typeProductRepository = typeProductRepository;
        }

        [HttpGet("GetTypesProduct")]
        public async Task<IActionResult> GetTypesProduct()
        {
            try
            {
                IEnumerable<TypeProductDto> typeProducts = await  _typeProductRepository.GetTypeProduct();
                return Ok(typeProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
