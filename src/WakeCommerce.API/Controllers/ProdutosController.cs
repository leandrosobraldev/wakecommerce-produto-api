using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Application.Interfaces;
using WakeCommerce.Domain.Common;

namespace WakeCommerce.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ProdutosController : Controller
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        //OK
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<ProdutoResponseDTO>>> Get([FromQuery] ProdutoSearchDTO produtoSearchDto, CancellationToken cancellationToken)
        {
            var result = await _produtoService.SearchAsync(produtoSearchDto, cancellationToken);
            return Ok(result);
        }

        //OK, MAS TENHO QUE TER MAIS IDEIA DO QUE PESQUISAR NA BUSCA.
        [HttpGet("{id:int}", Name = "GetProduto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoResponseDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _produtoService.GetById(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProdutoResponseDTO>> Post([FromBody] ProdutoCreateDTO produtoDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _produtoService.Add(produtoDto, cancellationToken);
            return CreatedAtRoute("GetProduto", new { id = result.Id }, result.Produto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] ProdutoUpdateDTO produtoDto, CancellationToken cancellationToken)
        {
            await _produtoService.Update(id, produtoDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _produtoService.Remove(id, cancellationToken);
            return NoContent();
        }
    }
}
