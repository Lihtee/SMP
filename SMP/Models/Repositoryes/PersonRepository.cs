using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.Models.Repositoryes
{
    public class PersonRepository
    {
        static ModelContainer cont = new ModelContainer();

        public PersonRepository(ModelContainer _cont)
        {
            cont = _cont;
        }

        /// <summary>
        /// Возвращает всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        public List<Person> GetPersons()
        {
            return cont.Person.OrderBy(p => p.IdPerson).ToList();
        }

        /// <summary>
        /// Возвращает пользователя по Id пользователя
        /// </summary>
        /// <param name="personId">Id пользователя</param>
        /// <returns>Пользователь</returns>
        public Person GetPersonById(int personId)
        {
            return cont.Person.SingleOrDefault(p => p.IdPerson == personId);
        }

        /// <summary>
        /// Возвращает пользователя по логин пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Пользователь</returns>
        public Person GetPersonByLogin(string login)
        {
            try
            {
                return cont.Person.Single(p => p.login == login);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        /// <summary>
        /// Добавляет пользователя в базу
        /// </summary>
        /// <param name="firstName">Фамилия</param>
        /// <param name="surName">Ммя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="position">Номер должности</param>
        /// <returns>Добавленный пользователь</returns>
        public Person AddPerson(string firstName, string surName, string middleName,
            string login, string password, int position, string email, string phone)
        {
            Person person = new Person
            {
                firstName = firstName,
                surName = surName,
                middleName = middleName,
                login = login,
                password = password.GetHashCode().ToString(),
                Position = (Position)position,
                email = email,
                phone = phone,
            };
            cont.Person.Add(person);
            cont.SaveChanges();
            return person;
        }

        /// <summary>
        /// Изменяет пользователя по Id пользователя
        /// </summary>
        /// <param name="personId">Id пользователя</param>
        /// <param name="firstName">Фамилия</param>
        /// <param name="surName">Ммя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="position">Номер должности</param>
        /// <returns>Изменённый пользователь</returns>
        public Person EditPerson(int personId, 
            string firstName, string surName, string middleName,
            string login, string password, int position, string email, string phone)
        {
            Person person = GetPersonById(personId);

            person.firstName = firstName;
            person.surName = surName;
            person.middleName = middleName;
            person.login = login;
            person.password = password;
            person.Position = (Position)position;
            person.email = email;
            person.phone = phone;
            
            cont.SaveChanges();
            return person;
        }

        /// <summary>
        /// Удаляет пользователя по Id пользователя
        /// </summary>
        /// <param name="personId">Id пользователя</param>
        public void DeletePerson(int personId)
        {
            cont.Person.Remove(GetPersonById(personId));
            cont.SaveChanges();
        }
    }
}