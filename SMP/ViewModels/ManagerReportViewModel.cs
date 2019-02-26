using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMP.Models;
using SMP.Models.Repositoryes;

namespace SMP.ViewModels
{
    /// <summary>
    /// Модель отчета о ходе проекта для личного кабинета менеджера.
    /// </summary>
    public class ManagerReportViewModel
    {
        public Project Project;

        public double CompletionRate;

        public int WorksCompleted;

        public int WorksInTotal;

        public double ReserveDaysRate;

        public int ReserveDaysUsed;

        public List<Project> TopLevelCompletedWorks;

        public ManagerReportViewModel(Project project)
        {
            var pr = new ProjectRepository(new ModelContainer());

            Project = project;
            var allTopChildren = pr.GetProjectsByParrentId(project.IdProject);
            var allLowestChildren = pr.GetLowestWorks(project);

            TopLevelCompletedWorks = allTopChildren.Where(proj => proj.isClose).ToList();

            WorksCompleted = allLowestChildren.Count(proj => proj.isClose);
            WorksInTotal = allLowestChildren.Count;
            CompletionRate = (double)WorksCompleted / WorksInTotal;

            var reserveStart = project.endDateTime - TimeSpan.FromDays(project.reserve);

            var intervals = new IntervalsSet();
            foreach (var topChild in allTopChildren)
            {
                if (topChild.endDateTime >= reserveStart)
                {
                    intervals.Add(new Interval
                    {
                        Strat = topChild.startDateTime >= reserveStart
                                        ? topChild.startDateTime
                                        : reserveStart,
                        End = topChild.endDateTime
                    }); 
                }
            }

            ReserveDaysUsed = intervals.Intervals.Sum(inter => (inter.End - inter.Strat).Value.Days + 1);
            ReserveDaysRate = (double)ReserveDaysUsed / project.reserve;
        }

        // Объявляю классы для работы с интервалами тут, чтобы было быстрее.

        /// <summary>
        /// Набор интервалов.
        /// </summary>
        private class IntervalsSet
        {
            internal List<Interval> Intervals;

            public IntervalsSet()
            {
                Intervals = new List<Interval>();
            }

            /// <summary>
            /// Добавить интервал в набор.
            /// </summary>
            public void Add(Interval interval)
            {
                Intervals.Add(interval);

                // Нормализовать полученный набор с учетом нового интервала.
                while (!NormalizeAddition()) { }
            } 

            /// <summary>
            /// Нормализовать набор интервалов при добавлении - склеить пересекающеся.
            /// При вызове набор должен быть уже нормальным.
            /// </summary>
            /// <returns></returns>
            private bool NormalizeAddition()
            {
                // Todo предусмотреть лучи - интервалы с одним концом.
                var toRemove = new List<Interval>();
                Interval newInterval = null;
                foreach (var lvlOne in Intervals)
                {
                    if (toRemove.Any())
                    {
                        break;
                    }

                    foreach (var lvlTwo in Intervals)
                    {
                        if (lvlOne != lvlTwo)
                        {
                            var primal = lvlOne;
                            var secondary = lvlTwo;
                            if (primal.Strat > secondary.Strat)
                            {
                                primal = lvlTwo;
                                secondary = lvlOne;
                            }

                            // Если пересекаются, то сплющиваем.
                            if (secondary.Strat <= primal.End)
                            {
                                newInterval = new Interval
                                {
                                    Strat = primal.Strat,
                                    End = primal.End > secondary.End
                                          ? primal.End
                                          : secondary.End
                                };
                                toRemove = new List<Interval> { primal, secondary };
                                break;
                            }
                        }
                    }
                }

                if (toRemove.Any())
                {
                    foreach (var inter in toRemove)
                    {
                        Intervals.Remove(inter);
                    }

                    Intervals.Insert(0, newInterval);

                    return false;
                }

                return true;
            }

            // Todo добавить метод для нормализации набора интервалов при вычитании.
        }

        private class Interval
        {
            public DateTime? Strat;

            public DateTime? End;
        }
    }
}