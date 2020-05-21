using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Api.Models;

namespace Core.Api.Applications
{
    public interface IStudentRepository
    {
        List<Student> List();

        Student Get(int id);

        bool Add(Student student);

        bool Update(Student student);

        bool Delete(int id);
    }
}
