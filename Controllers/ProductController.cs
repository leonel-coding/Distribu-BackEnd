using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using DTOLibrary.ViewModels;

namespace BackEnd_ADONET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                IEnumerable<ProductDto> products = await _productRepository.GetProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetProductWithRelationsSuppliersByProductId")]
        public async Task<IActionResult> GetProductWithRelationsSuppliersByProductId(int productId)
        {
            try
            {
                ProductViewModel product = await _productRepository.GetProductWithRelationsSuppliersByProductId(productId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                ProductDto product = await _productRepository.GetProductById(productId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertProduct([FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var productResult = await _productRepository.InsertProduct(product);

                return Ok(productResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar el producto: {ex.Message}");
            }
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto product)
        {
            try
            {
                await _productRepository.UpdateProduct(product);

                return Ok("Se actualizo correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el producto: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {

            try
            {
                await _productRepository.DeleteProduct(productId);

                return Ok("Producto y relaciones desactivados correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al desactivar el producto y sus relaciones: {ex.Message}");
            }
        }
    }
}
