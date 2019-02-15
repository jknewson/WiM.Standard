//------------------------------------------------------------------------------
//----- JoinCollectionFacade ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   

//    authors:  Artheur J Vickers
//               
//  
//   purpose:   Handles many-to-many relationships
//
//discussion:   Handles many-to-many relationships by abstracting away some methods
//              to make it easier to work with m2m relationships
//              https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-1-the-basics/
//              https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-2-hiding-as-ienumerable/
//              https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-3-hiding-as-icollection/
//              https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-4-a-more-general-abstraction/
// 
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WIM.Utilities
{
    public interface IJoinEntity<TEntity>
    {
        TEntity Navigation { get; set; }
    }

    public class JoinCollectionFacade<TEntity, TOtherEntity, TJoinEntity>
    : ICollection<TEntity>
    where TJoinEntity : IJoinEntity<TEntity>, IJoinEntity<TOtherEntity>, new()
    {
        private readonly TOtherEntity _ownerEntity;
        private readonly ICollection<TJoinEntity> _collection;

        public JoinCollectionFacade(
            TOtherEntity ownerEntity,
            ICollection<TJoinEntity> collection)
        {
            _ownerEntity = ownerEntity;
            _collection = collection;
        }

        public IEnumerator<TEntity> GetEnumerator()
            => _collection.Select(e => ((IJoinEntity<TEntity>)e).Navigation).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinEntity<TEntity>)entity).Navigation = item;
            ((IJoinEntity<TOtherEntity>)entity).Navigation = _ownerEntity;
            _collection.Add(entity);
        }

        public void Clear()
            => _collection.Clear();

        public bool Contains(TEntity item)
            => _collection.Any(e => Equals(item, e));

        public void CopyTo(TEntity[] array, int arrayIndex)
            => this.ToList().CopyTo(array, arrayIndex);

        public bool Remove(TEntity item)
            => _collection.Remove(
                _collection.FirstOrDefault(e => Equals(item, e)));

        public int Count
            => _collection.Count;

        public bool IsReadOnly
            => _collection.IsReadOnly;

        private static bool Equals(TEntity item, TJoinEntity e)
            => Equals(((IJoinEntity<TEntity>)e).Navigation, item);

    }
}
