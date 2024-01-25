using StudiaZadanko.Models;
using System.Collections.Generic;

namespace StudiaZadanko.Interfaces;

public interface IPersonRepository
{
    void Add(Person person);
    IEnumerable<Person> GetAll();
    void Update(Person person);
    void Delete(int id);
}
