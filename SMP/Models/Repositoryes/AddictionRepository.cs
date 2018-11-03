using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.Models.Repositoryes
{
    public class AddictionRepository
    {
        static ModelContainer cont = new ModelContainer();

        private readonly ProjectRepository ProjectRepository;

        public AddictionRepository(ModelContainer _cont)
        {
            cont = _cont;
            ProjectRepository = new ProjectRepository(cont);
        }

        /// <summary>
        /// Возвращает все зависимости
        /// </summary>
        /// <returns>Список зависимостей</returns>
        public List<Addiction> GetAddictions()
        {
            return cont.Addiction.OrderBy(p => p.Id).ToList();
        }

        /// <summary>
        /// Возвращает зависимость по Id зависимости
        /// </summary>
        /// <param name="Id">Id зависимости</param>
        /// <returns>Зависимость</returns>
        public Addiction GetAddictionById(int Id)
        {
            return cont.Addiction.SingleOrDefault(p => p.Id == Id);
        }

        /// <summary>
        /// Возвращает зависимость по Id проектов
        /// </summary>
        /// <param name="lastProjectId">Id предшествующего проекта</param>
        /// <param name="nextProjectId">Id следующего проекта</param>
        /// <returns>Зависимость</returns>
        public Addiction GetAddictionByProjectsId(int lastProjectId, int nextProjectId)
        {
            return cont.Addiction.SingleOrDefault(p => p.lastProject.IdProject == lastProjectId && p.nextProject.IdProject == nextProjectId);
        }

        /// <summary>
        /// Возвращает зависимость по Id предшествующего проекта
        /// </summary>
        /// <param name="Id">Id предшествующего проекта</param>
        /// <returns>Список зависимостей</returns>
        public List<Addiction> GetAddictionsByLastProjectID(int Id)
        {
            return cont.Addiction.OrderBy(p => p.lastProject.IdProject == Id).ToList();
        }

        /// <summary>
        /// Возвращает зависимость по Id следующего проекта
        /// </summary>
        /// <param name="Id">Id следующего проекта</param>
        /// <returns>Список зависимостей</returns>
        public List<Addiction> GetAddictionsByNextProjectID(int Id)
        {
            return cont.Addiction.OrderBy(p => p.nextProject.IdProject == Id).ToList();
        }

        /// <summary>
        /// Добавляет зависимость в базу
        /// </summary>
        /// <param name="lastProjectId">Id предшествующего проекта</param>
        /// <param name="nextProjectId">Id следующего проекта</param>
        /// <returns>Добавленная зависимость</returns>
        public Addiction AddAddiction(int lastProjectId, int nextProjectId)
        {
            Addiction addiction = new Addiction
            {
                lastProject = cont.Project.SingleOrDefault(p => p.IdProject == lastProjectId),
                nextProject = cont.Project.SingleOrDefault(p => p.IdProject == nextProjectId),
            };
            cont.Addiction.Add(addiction);
            cont.SaveChanges();
            return addiction;
        }

        /// <summary>
        /// Изменяет зависимость по Id зависимости
        /// </summary>
        /// <param name="addictiontId">Id зависимости</param>
        /// <param name="lastProjectId">Id предшествующего проекта</param>
        /// <param name="nextProjectId">Id следующего проекта</param>
        /// <returns>Изменённая зависимость</returns>
        public Addiction EditAddiction(int addictiontId, int lastProjectId, int nextProjectId)
        {
            Addiction addictiont = GetAddictionById(addictiontId);

            addictiont.lastProject = cont.Project.SingleOrDefault(p => p.IdProject == lastProjectId);
            addictiont.lastProject = cont.Project.SingleOrDefault(p => p.IdProject == nextProjectId);
            
            cont.SaveChanges();
            return addictiont;
        }

        /// <summary>
        /// Удаляет зависимость по Id зависимости
        /// </summary>
        /// <param name="addictiontId">Id зависимости</param>
        public void DeleteAddiction(int addictiontId)
        {
            cont.Addiction.Remove(GetAddictionById(addictiontId));
            cont.SaveChanges();
        }

        #region PeriodAmending

        /// <summary>
        /// Изменяет сроки работы. 
        /// </summary>
        /// <param name="project">Еще не сохраненная в БД работа.</param>
        /// <returns>Измененные проекты.</returns>
        public void AmendPeriod(Project project)
        {
            DoRecShift(project, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="pushChildren">Надо ли толкать нижние работы (ставить в false, если метод вызывается для родительской работы)</param>
        /// <returns></returns>
        private void DoRecShift(Project current, bool pushChildren)
        {
            if (pushChildren)
            {
                var old = ProjectRepository.GetProjectById(current.IdProject);
                // Сначала двигаем нижние работы.
                // Допущение о том, что работы, имеющие потомков, не изменяют свою длительность. 
                var shift = current.endDateTime - old.endDateTime;
                MoveChildren(current, shift);

            }

            // Толкаем следующие работы.
            var nexts = GetAddictionsByLastProjectID(current.IdProject).Select(x => x.nextProject).ToList();
            if (nexts.Any())
            {
                foreach (var next in nexts)
                {
                    if (ForvardPropagation(current, next))
                    {
                        DoRecShift(next, true);
                    }
                }
            }
            else
            {
                // Пробуем толкнуть верхнюю работу самой поздней нижней. 
                var latest = ProjectRepository
                    .GetProjectsByParrentId(current.parrentProject?.IdProject ?? -1)
                    .OrderByDescending(x => x.endDateTime)
                    .FirstOrDefault();
                if (latest?.IdProject == current.IdProject && UpperPropagation(current, current.parrentProject, true))
                {
                    DoRecShift(current.parrentProject, false);
                }
            }

            //Толкаем предыдущие работы. 
            var prevs = GetAddictionsByNextProjectID(current.IdProject).Select(x => x.lastProject).ToList();
            if (prevs.Any())
            {
                foreach (var prev in prevs)
                {
                    if (BackwardPropagation(current, prev))
                    {
                        DoRecShift(prev, true);
                    }
                }
            }
            else
            {
                // Пробуем толкнуть верхнюю работу самой ранней нижней.
                var earliest = ProjectRepository
                    .GetProjectsByParrentId(current.parrentProject?.IdProject ?? -1)
                    .OrderBy(x => x.startDateTime)
                    .FirstOrDefault();
                if (earliest?.IdProject == current.IdProject &&
                    UpperPropagation(current, current.parrentProject, false))
                {
                    DoRecShift(current.parrentProject, false);
                }
            }
        }

        /// <summary>
        /// Распространяет изменения на следущую работу.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private bool ForvardPropagation(Project prev, Project next)
        {
            bool res = false;
            var pushShift = prev.endDateTime - next.startDateTime;
            var nextDur = next.endDateTime - next.startDateTime;

            // Толкаем работу. 
            if (pushShift.TotalMilliseconds > 0)
            {
                next.startDateTime = prev.endDateTime;
                next.endDateTime = next.startDateTime + nextDur;
                res = true;
            }
            
            return res;
        }

        /// <summary>
        /// Распространяет изменения на предыдущую работу.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private bool BackwardPropagation(Project next, Project prev)
        {
            bool res = false;
            var pushShift = prev.endDateTime - next.startDateTime;
            var prevDur = next.endDateTime - next.startDateTime;

            // Толкаем или тянем работу. 
            if (pushShift.TotalMilliseconds < 0)
            {
                prev.endDateTime = next.startDateTime;
                prev.startDateTime = prev.endDateTime - prevDur;
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Толкает верхнюю работу. 
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="higher"></param>
        /// <returns></returns>
        private bool UpperPropagation(Project lower, Project higher, bool last)
        {
            if (higher == null)
            {
                return false;
            }

            bool res = false;
            if (last && lower.endDateTime != higher.endDateTime)
            {
                higher.endDateTime = lower.endDateTime;
                res = true;
            }

            if (!last && lower.startDateTime != higher.startDateTime)
            {
                higher.startDateTime = lower.startDateTime;
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Двигает все нижние работы.
        /// </summary>
        /// <param name="shift"></param>
        private void MoveChildren(Project root, TimeSpan shift)
        {
            var children = ProjectRepository.GetProjectsByParrentId(root.IdProject);
            foreach (var child in children)
            {
                child.startDateTime += shift;
                child.endDateTime += shift;
                MoveChildren(child, shift);
            }
        }
        #endregion
    }
}