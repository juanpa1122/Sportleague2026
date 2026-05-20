using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly MatchValidationHelper _validationHelper;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchRepository matchRepository,
            IPlayerRepository playerRepository,
            IMatchLineupRepository matchLineupRepository,
            MatchValidationHelper validationHelper,
            ILogger<MatchLineupService> logger)
        {
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
            _matchLineupRepository = matchLineupRepository;
            _validationHelper = validationHelper;
            _logger = logger;
        }

        public async Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

            var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);
            if (player == null)
                throw new KeyNotFoundException($"No se encontró el jugador con ID {lineup.PlayerId}");

            await _validationHelper.ValidatePlayerInMatchAsync(lineup.PlayerId, match);

            if (await _matchLineupRepository.ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId))
                throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

            if (lineup.IsStarter)
            {
                var starters = await _matchLineupRepository.CountStartersByMatchAndTeamAsync(matchId, player.TeamId);
                if (starters >= 11)
                    throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
            }

            lineup.MatchId = matchId;
            lineup.Position = lineup.Position?.Trim() ?? string.Empty;

            _logger.LogInformation(
                "Adding player {PlayerId} to lineup for match {MatchId}. Starter: {IsStarter}",
                lineup.PlayerId, matchId, lineup.IsStarter);

            var created = await _matchLineupRepository.CreateAsync(lineup);
            return (await _matchLineupRepository.GetByMatchAsync(matchId))
                .First(ml => ml.Id == created.Id);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            return await _matchLineupRepository.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            if (teamId != match.HomeTeamId && teamId != match.AwayTeamId)
                throw new InvalidOperationException("El equipo indicado no pertenece a este partido");

            return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task DeleteAsync(int matchId, int id)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            var lineup = (await _matchLineupRepository.GetByMatchAsync(matchId)).FirstOrDefault(ml => ml.Id == id);
            if (lineup == null)
                throw new KeyNotFoundException($"No se encontró el registro de alineación con ID {id}");

            await _matchLineupRepository.DeleteAsync(id);
        }
    }
}
