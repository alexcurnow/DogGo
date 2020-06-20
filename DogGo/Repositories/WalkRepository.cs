using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetAllWalks()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Date, Duration, WalkerId, DogId
                        FROM Walks
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId"))
                        };

                        walks.Add(walk);
                    }

                    reader.Close();

                    return walks;
                }
            }
        }

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id as WalkId, w.Date as WalkDate, w.Duration as WalkDuration, w.WalkerId as WalkerId, w.DogId as DogId,
                        o.Id as OwnerId, o.Email as OwnerEmail, o.Name as OwnerName, o.Address as OwnerAddress, o.NeighborhoodId as OwnerNeighborhoodId, o.Phone as OwnerPhone
                        FROM Walks w
                        LEFT JOIN DOG d on w.DogId = d.Id
                        LEFT JOIN Owner o on d.OwnerId = o.Id
                        WHERE WalkerId = @walkerId
                    ";

                    cmd.Parameters.AddWithValue("@walkerId", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    
                    while (reader.Read())
                    {
                        Owner owner = new Owner()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Email = reader.GetString(reader.GetOrdinal("OwnerEmail")),
                            Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                            Address = reader.GetString(reader.GetOrdinal("OwnerAddress")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("OwnerNeighborhoodId")),
                            Phone = reader.GetString(reader.GetOrdinal("OwnerPhone"))
                        };

                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                            Date = reader.GetDateTime(reader.GetOrdinal("WalkDate")),
                            Duration = reader.GetInt32(reader.GetOrdinal("WalkDuration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Owner = owner
                        };
                        walks.Add(walk);


                    }
                        reader.Close();
                        return walks;
                    
                }
            }
        }
    }
}
