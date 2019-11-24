using System;
using System.Collections.Generic;

namespace ReusableServices.Common.Extensions
{
  public static class CollectionExtension
  {
    /// <summary>
    /// Позволяет получить текущий индекс bunch и общее количество bunch-ей. Выполняется до итерирования по bunch-у.
    /// </summary>
    /// <param name="bunchIndex">Индекс текущего bunch</param>
    /// /// <param name="bunchesCount">Количество всего bunch-ей</param>
    public delegate void InfoAction(int bunchIndex, int bunchesCount);

    /// <summary>
    ///   Выполняет функцию на группой элементов из коллекции, а не сразу над всеми
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">Список элементов</param>
    /// <param name="bunchSize">Размер bunch</param>
    /// <param name="bunchAction">Действие, выполняемое над каждым bunch</param>
    /// <returns>ICollection<typeparamref name="T" /></returns>
    public static ICollection<T> SelectByBunch<T>(this IList<T> list, int bunchSize, Action<T> bunchAction)
    {
      return SelectByBunch(list, bunchSize, bunchAction, (index, count) => { }, () => { });
    }

    /// <summary>
    ///   Выполняет функцию на группой элементов из коллекции, а не сразу над всеми
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">Список элементов</param>
    /// <param name="bunchSize">Размер bunch</param>
    /// <param name="bunchAction">Действие, выполняемое над каждым bunch</param>
    /// <param name="beforeBunchAction">Позволяет получить текущий индекс bunch и общее количество bunch-ей</param>
    /// <param name="afterBunchAction">Выполняется после каждого bunch</param>
    /// <returns>ICollection<typeparamref name="T" /></returns>
    public static ICollection<T> SelectByBunch<T>(this IList<T> list, int bunchSize, Action<T> bunchAction, InfoAction beforeBunchAction, Action afterBunchAction)
    {
      if (bunchSize <= 0) throw new ArgumentException("Bunch size has to be positive");
      var count = (int)Math.Ceiling((double)list.Count / bunchSize);
      for (var i = 0; i < count; i++)
      {
        beforeBunchAction(i, count);
        var shortestSize = (i + 1) * bunchSize < list.Count ? (i + 1) * bunchSize : list.Count;
        for (var j = i * bunchSize; j < shortestSize; j++)
        {
          bunchAction(list[j]);
        }

        afterBunchAction();
      }
      return list;
    }
  }
}