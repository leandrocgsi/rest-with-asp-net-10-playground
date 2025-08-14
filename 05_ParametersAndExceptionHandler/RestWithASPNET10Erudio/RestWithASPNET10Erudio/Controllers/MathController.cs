using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Services;
using RestWithASPNET10Erudio.Utils;

namespace RestWithASPNET10Erudio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MathController : ControllerBase
    {

        private readonly MathService _mathService;

        public MathController(MathService mathService)
        {
            _mathService = mathService;
        }

        [HttpGet("sum/{firstNumber}/{secondNumber}")]
        public IActionResult Get(string firstNumber, string secondNumber)
        {
            if (NumberHelper.IsNumeric(firstNumber) && NumberHelper.IsNumeric(secondNumber))
            {
                var sum = _mathService.Sum(
                    NumberHelper.ConvertToDecimal(firstNumber),
                    NumberHelper.ConvertToDecimal(secondNumber));
                return Ok(sum.ToString());
            }

            return BadRequest("Invalid Input");
        }

        [HttpGet("subtraction/{firstNumber}/{secondNumber}")]
        public IActionResult Subtraction(string firstNumber, string secondNumber)
        {
            if (NumberHelper.IsNumeric(firstNumber) && NumberHelper.IsNumeric(secondNumber))
            {
                var subtraction = _mathService.Subtract(
                    NumberHelper.ConvertToDecimal(firstNumber),
                    NumberHelper.ConvertToDecimal(secondNumber));
                return Ok(subtraction.ToString());
            }

            return BadRequest("Invalid Input");
        }

        [HttpGet("division/{firstNumber}/{secondNumber}")]
        public IActionResult Division(string firstNumber, string secondNumber)
        {
            if (NumberHelper.IsNumeric(firstNumber) && NumberHelper.IsNumeric(secondNumber))
            {
                var division = _mathService.Division(
                    NumberHelper.ConvertToDecimal(firstNumber),
                    NumberHelper.ConvertToDecimal(secondNumber)
                );
                return Ok(division.ToString());
            }
            return Ok("Invalid Input");
        }

        [HttpGet("multiplication/{firstNumber}/{secondNumber}")]
        public IActionResult Multiplication(string firstNumber, string secondNumber)
        {
            if (NumberHelper.IsNumeric(firstNumber) && NumberHelper.IsNumeric(secondNumber))
            {
                var multiplication = _mathService.Multiply(
                    NumberHelper.ConvertToDecimal(firstNumber),
                    NumberHelper.ConvertToDecimal(secondNumber)
                );
                return Ok(multiplication.ToString());
            }
            return Ok("Invalid Input");
        }

        [HttpGet("mean/{firstNumber}/{secondNumber}")]
        public IActionResult Mean(string firstNumber, string secondNumber)
        {
            if (NumberHelper.IsNumeric(firstNumber) && NumberHelper.IsNumeric(secondNumber))
            {
                var mean = _mathService.Mean(
                    NumberHelper.ConvertToDecimal(firstNumber),
                    NumberHelper.ConvertToDecimal(secondNumber)
                );
                return Ok(mean.ToString());
            }
            return Ok("Invalid Input");
        }

        [HttpGet("square-root/{number}")]
        public IActionResult SquareRoot(string number)
        {
            if (NumberHelper.IsNumeric(number))
            {
                var squareRoot = _mathService.SquareRoot(
                    NumberHelper.ConvertToDecimal(number)
                    );
                return Ok(squareRoot.ToString());
            }
            return Ok("Invalid Input");
        }
    }
}
