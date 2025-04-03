using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAppProducto.Context;
using BackendAppProducto.Models;

namespace BackendAppProducto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Listado de todos los productos
        /// </summary>
        /// <returns>Lista de productos</returns>
        // GET: api/Products
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Lista un solo producto por su Id
        /// </summary>
        /// <param name="id">El id del producto a mostrar</param>
        /// <returns>Un solo producto</returns>
        // GET: api/Products/5
        [HttpGet("list/{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Producto no encontrado.");
            }

            return product;
        }

        /// <summary>
        /// Modifica un producto según su Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        // PUT: api/Products/5
        [HttpPut("modify/{id}")]
        public async Task<IActionResult> PutProduct(Guid id, [FromBody] Product product)
        {
            // Validaciones de datos de entrada
            if (id != product.Id)
            {
                return BadRequest("El id del producto no coincide con el producto que se está intentando modificar.");
            }

            if (string.IsNullOrEmpty(product.Name))
            {
                return BadRequest("El nombre del producto no puede estar vacío.");
            }

            if (product.Price <= 0)
            {
                return BadRequest("El precio debe ser mayor que 0.");
            }

            if (product.Stock < 0)
            {
                return BadRequest("El stock no puede ser negativo.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound("Producto no encontrado.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="product">Datos del nuevo producto</param>
        /// <returns></returns>
        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            // Validaciones de datos de entrada
            if (product == null)
            {
                return BadRequest("El producto no puede ser nulo.");
            }

            if (string.IsNullOrEmpty(product.Name))
            {
                return BadRequest("El nombre del producto no puede estar vacío.");
            }

            if (product.Price <= 0)
            {
                return BadRequest("El precio debe ser mayor que 0.");
            }

            if (product.Stock < 0)
            {
                return BadRequest("El stock no puede ser negativo.");
            }

            product.Id = Guid.NewGuid(); // Nos aseguramos que se genere un GUID antes de guardarlo
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        /// <summary>
        /// Elimina un producto según su Id
        /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        // DELETE: api/Products/5
        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Producto no encontrado.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
