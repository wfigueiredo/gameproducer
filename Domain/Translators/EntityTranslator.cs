﻿using GameProducer.Util;
using System.Collections.Generic;
using System.Linq;
using GameProducer.Domain.DTO;
using GameProducer.Domain.Model;
using GameProducer.Domain.Enum;

namespace GameProducer.Domain.Translators
{
    public static class EntityTranslator
    {
        public static Game ToDomain(this IGDBNextGameRelease release)
        {
            if (release != null)
            {
                System.Enum.TryParse(release.platform.abbreviation.ToLower(), true, out ConsoleType consoleType);
                return new Game()
                {
                    externalId = release.game.id,
                    name = release.game.name,
                    summary = release.game.summary,
                    publisher = GetPublisherName(release.game.involved_companies),
                    releaseDate = release.date.FromUnixTimeStampToDateTime(),
                    consoleType = consoleType,
                    consoleAbbreviation = release.platform.abbreviation.ToLower(),
                    consoleName = release.platform.name
                };
            }

            return null;
        }

        public static IList<Game> ToDomainList(this IEnumerable<IGDBNextGameRelease> releases)
        {
            return releases?.Select(ToDomain).ToList();
        }

        private static string GetPublisherName(IEnumerable<IGDBInvolvedCompany> involved_companies)
        {
            return involved_companies
                .Select(c => new { c.publisher, c.company })
                .Where(bundle => bundle.publisher)
                .FirstOrDefault()
                .company.name;
        }
    }
}