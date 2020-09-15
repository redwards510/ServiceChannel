using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;

namespace ServiceChannel.Model
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            using var reader = new StreamReader("Data/time_series_covid19_confirmed_US.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();

            csv.ReadHeader();
            var headerLength = csv.Context.ColumnCount;
            var countyId = 1;
            var infectionId = 1;
            while (csv.Read())
            {
                modelBuilder.Entity<County>().HasData(
                    new County
                    {
                        ID = countyId,
                        UID = csv.GetField<int>(0),
                        Iso2 = csv.GetField(1),
                        Iso3 = csv.GetField(2),
                        Code3 = csv.GetField(3),
                        FIPS = csv.GetField(4),
                        Name = csv.GetField(5),
                        State = csv.GetField(6),
                        Country = csv.GetField(7),
                        Lat = csv.GetField<double>(8),
                        Long = csv.GetField<double>(9),
                        Combined_Key = csv.GetField(10)
                    }
                );

                // for very first day of input we assume that day is all new cases.
                int newCases = 0;
                for (int i = 11; i < csv.Context.HeaderRecord.Length; i++)
                {
                    modelBuilder.Entity<Infections>().HasData(
                        new Infections
                        {
                            ID = infectionId,
                            CountyID = countyId,
                            Count = csv.GetField<int>(i),
                            Date = Convert.ToDateTime(csv.Context.HeaderRecord[i]),
                            NewCases = csv.GetField<int>(i) - newCases
                        });
                    infectionId++;
                    newCases = csv.GetField<int>(i);
                }

                countyId++;
            }
        }
    }
}
