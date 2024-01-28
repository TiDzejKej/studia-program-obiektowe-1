using MySql.Data.MySqlClient;
using StudiaZadanko.Interfaces;
using StudiaZadanko.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace StudiaZadanko.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        public void Add(Person person)
        {
            try
            {
                DbConnection.Instance.ExecuteInsert(person);
            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                DbConnection.Instance.ExecuteDelete(id, "persons");
            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public IEnumerable<Person> GetAll()
        {
            try
            {
                return DbConnection.Instance.ExecuteGetAll<Person>("persons");
            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
                throw;
            }
        }

      
     
    }
}
