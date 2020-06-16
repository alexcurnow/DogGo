using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class OwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
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

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, NeighborhoodId, Phone
                        FROM Owner
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone"))
                        };

                        owners.Add(owner);
                    }

                    reader.Close();

                    return owners;
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id as OwnerId, o.[Name] as OwnerName, o.Email as OwnerEmail, o.Address as OwnerAddress, o.NeighborhoodId as OwnerNeighborhoodId, o.Phone as OwnerPhone, d.Name as DogName, d.Breed as DogBreed
                        FROM Owner o
                        JOIN Dog d on d.OwnerId = o.Id
                        WHERE o.Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    Owner owner = null;

                    while (reader.Read())
                    {
                        if (owner == null)
                        {
                        owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                            Email = reader.GetString(reader.GetOrdinal("OwnerEmail")),
                            Address = reader.GetString(reader.GetOrdinal("OwnerAddress")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("OwnerNeighborhoodId")),
                            Phone = reader.GetString(reader.GetOrdinal("OwnerPhone"))
                        };

                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("DogName")))
                        {
                            owner.Dogs.Add(new Dog()
                        {
                            Name = reader.GetString(reader.GetOrdinal("DogName")),
                            Breed = reader.GetString(reader.GetOrdinal("DogBreed"))
                        });

                        }

                      

                    }
                        reader.Close();
                        return owner;
                    
                }
            }
        }
    }
}