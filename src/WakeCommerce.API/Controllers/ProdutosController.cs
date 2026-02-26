using Microsoft.AspNetCore.Mvc;
using WakeCommerce.Application.DTO;
using WakeCommerce.Application.Interfaces;

namespace WakeCommerce.API.Controllers
{
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class ProdutosController : Controller
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] string? name)
        {
            if (!string.IsNullOrEmpty(name))
                return Ok(await _produtoService.GetByName(name));

            return Ok(await _produtoService.GetProdutos());
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _produtoService.Add(produtoDto);

            return new CreatedAtRouteResult("GetProduto",
                new { id = produtoDto.Id }, produtoDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
        {
            if (id != produtoDto.Id)
            {
                return BadRequest();
            }

            await _produtoService.Update(produtoDto);

            return Ok(produtoDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produtoDto = await _produtoService.GetById(id);
            if (produtoDto == null)
            {
                return NotFound();
            }
            await _produtoService.Remove(id);
            return Ok(produtoDto);
        }
    }
}
