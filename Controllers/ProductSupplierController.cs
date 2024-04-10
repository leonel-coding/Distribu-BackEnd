using BackEnd_ADONET.Repositories;
using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_ADONET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSupplierController : ControllerBase
    {
        private readonly IProductSupplierRepository _productSupplierRepository;

        public ProductSupplierController(IProductSupplierRepository productSupplierRepository)
        {
            _productSupplierRepository = productSupplierRepository;
        }


        [HttpGet("GetProductSupplierById")]
        public async Task<IActionResult> GetProductSupplierById(int productSupplierId)
        {
            try
            {
                ProductSupplierDto productSupplier = await _productSupplierRepository.GetProductSupplierById(productSupplierId);
                return Ok(productSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertProductSupplier([FromBody] ProductSupplierDto productSupplier)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var productSupplierResult = await _productSupplierRepository.InsertProductSupplier(productSupplier);

                return Ok(productSupplierResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar el producto: {ex.Message}");
            }
        }


        [HttpPut("UpdateProductSupplier")]
        public async Task<IActionResult> UpdateProductSupplier([FromBody] ProductSupplierDto productSupplier)
        {
            try
            {
                await _productSupplierRepository.UpdateProductSupplier(productSupplier);

                return Ok("Se actualizó correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("DeleteProductSupplier")]
        public async Task<IActionResult> DeleteProductSupplier(int productSupplierId)
        {

            try
            {
                await _productSupplierRepository.DeleteProductSupplier(productSupplierId);

                return Ok("Desactivado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al desactivar el producto y sus relaciones: {ex.Message}");
            }
        }
    }
}
