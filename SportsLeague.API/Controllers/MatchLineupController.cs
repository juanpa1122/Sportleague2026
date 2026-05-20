using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}/lineup")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(IMatchLineupService matchLineupService, IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MatchLineupDto>> Create(int matchId, CreateMatchLineupDto dto)
        {
            try
            {
                var entity = _mapper.Map<MatchLineup>(dto);
                var created = await _matchLineupService.AddPlayerAsync(matchId, entity);
                var response = _mapper.Map<MatchLineupDto>(created);
                return Created($"/api/match/{matchId}/lineup/{response.Id}", response);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetByMatch(int matchId)
        {
            try
            {
                var lineup = await _matchLineupService.GetByMatchAsync(matchId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupDto>>(lineup));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetByMatchAndTeam(int matchId, int teamId)
        {
            try
            {
                var lineup = await _matchLineupService.GetByMatchAndTeamAsync(matchId, teamId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupDto>>(lineup));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int matchId, int id)
        {
            try
            {
                await _matchLineupService.DeleteAsync(matchId, id);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }
    }
}
