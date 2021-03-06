﻿
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameClientApi.Domain.DTO;
using GameClientApi.Domain.Enum;
using GameClientApi.Interfaces.Error;
using GameClientApi.Interfaces.Services;
using GameClientApi.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameClientApi.Controllers
{
    [Route("api/gameclient/v1/game")]
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;

        public GameController(ILogger<GameController> logger, IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet("releases")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetReleases(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var nextReleases = await _gameService.fetchGameReleases(startDate, endDate, maxResults);
                return Ok(nextReleases);
            }
            catch (GenericApiException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "Invalid request",  reason = ex.Message});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal error on Publisher API", reason =  ex.Message});
            }
        }

        [HttpPost("releases/monthly-report")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GenerateMonthlyReleasesReport([FromBody] ReportDto reportDto)
        {
            try
            {
                await _gameService.ExportMonthlyReleases(reportDto);
                return Accepted();
            }
            catch (GenericApiException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "Invalid request", reason = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal error on Publisher API", reason = ex.Message });
            }
        }
    }
}
