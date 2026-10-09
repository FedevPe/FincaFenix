namespace FincaFenix.Entities.Rendimiento
{
    public static class RendimientoCalculator
    {
        public static decimal? TheoreticalMachinePasses(decimal areaTotal, decimal trv, decimal volumeMachine)
        {
            if (areaTotal <= 0 || trv <= 0 || volumeMachine <= 0)
            {
                return null;
            }

            return areaTotal * trv / volumeMachine;
        }

        public static decimal? MaterialEfficiency(decimal areaTotal, decimal trv, decimal volumeMachine, decimal realMachinePasses)
        {
            var theoretical = TheoreticalMachinePasses(areaTotal, trv, volumeMachine);

            if (theoretical is null or <= 0 || realMachinePasses <= 0)
            {
                return null;
            }

            return theoretical.Value / realMachinePasses * 100m;
        }

        public static decimal? EfficiencyFromAmounts(decimal? theoreticalAmount, decimal realAmount)
        {
            if (theoreticalAmount is null || theoreticalAmount <= 0 || realAmount <= 0)
            {
                return null;
            }

            return theoreticalAmount.Value / realAmount * 100m;
        }

        public static decimal? PerManHour(decimal? output, decimal manHours)
        {
            if (output is null || manHours <= 0)
            {
                return null;
            }

            return output.Value / manHours;
        }

        public static decimal? ManHours(decimal manHours)
        {
            return manHours > 0 ? manHours : null;
        }
    }
}
