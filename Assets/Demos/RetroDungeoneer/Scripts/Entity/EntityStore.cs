namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Singleton entity storage class, responsible for storing all entities and creating new ones
    /// </summary>
    public class EntityStore
    {
        private static EntityStore mInstance;

        private List<EntityID> mActiveEntities = new List<EntityID>();
        private List<Entity> mEntities = new List<Entity>();
        private List<EntityID> mEntityIDs = new List<EntityID>();
        private List<int> mFreeIndecies = new List<int>();
        private int mCapacity = 0;
        private int mLastFreeIndex = 0;

        private EntityStore(int initialCapacity)
        {
            Initialize(initialCapacity);
        }

        /// <summary>
        /// Get the EntityStore instance
        /// </summary>
        public static EntityStore Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new EntityStore(1024);
                }

                return mInstance;
            }
        }

        /// <summary>
        /// Get the entities in this store
        /// </summary>
        public static List<EntityID> entities
        {
            get
            {
                return Instance.mActiveEntities;
            }
        }

        /// <summary>
        /// Clear the entity store
        /// </summary>
        public static void Clear()
        {
            Instance.Initialize(Instance.mCapacity);
        }

        /// <summary>
        /// Create a new empty entity
        /// </summary>
        /// <returns>New entity</returns>
        public static EntityID CreateEntity()
        {
            // No more indecies!
            if (Instance.mFreeIndecies[0] == -1)
            {
                // Return dummy ID
                return new EntityID(0, 0);
            }

            Instance.mEntityIDs[Instance.mFreeIndecies[0]].NextGen();
            EntityID id = Instance.mEntityIDs[Instance.mFreeIndecies[0]];

            // Remove the free index by swapping with last free one
            Instance.mFreeIndecies[0] = Instance.mFreeIndecies[Instance.mLastFreeIndex];
            Instance.mFreeIndecies[Instance.mLastFreeIndex] = -1;
            Instance.mLastFreeIndex--;

            Instance.mEntities[id.index] = new Entity(id);

            Instance.mActiveEntities.Add(id);

            return id;
        }

        /// <summary>
        /// Destroy an entity
        /// </summary>
        /// <param name="id">Entity id</param>
        public static void DestroyEntity(EntityID id)
        {
            Instance.mLastFreeIndex++;
            Instance.mFreeIndecies[Instance.mLastFreeIndex] = id.index;

            Instance.mEntities[id.index] = null;

            Instance.mActiveEntities.Remove(id);
        }

        /// <summary>
        /// Clear all entities except the listed ones
        /// </summary>
        /// <param name="exceptions">Exception list</param>
        public static void ClearExcept(List<EntityID> exceptions)
        {
            List<EntityID> deleteList = new List<EntityID>();

            for (int i = 0; i < Instance.mActiveEntities.Count; i++)
            {
                var id = Instance.mActiveEntities[i];
                if (!exceptions.Contains(id))
                {
                    deleteList.Add(id);
                }
            }

            for (int i = 0; i < deleteList.Count; i++)
            {
                DestroyEntity(deleteList[i]);
            }
        }

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Entity</returns>
        public static Entity Get(EntityID id)
        {
            var checkId = Instance.mEntityIDs[id.index];
            if (checkId == id)
            {
                return Instance.mEntities[id.index];
            }

            return null;
        }

        /// <summary>
        /// Write entity store
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public static void Write(BinaryWriter writer)
        {
            writer.Write(Instance.mEntityIDs.Count);
            for (int i = 0; i < Instance.mEntityIDs.Count; i++)
            {
                Instance.mEntityIDs[i].Write(writer);
            }

            writer.Write(Instance.mFreeIndecies.Count);
            for (int i = 0; i < Instance.mFreeIndecies.Count; i++)
            {
                writer.Write(Instance.mFreeIndecies[i]);
            }

            writer.Write(Instance.mEntities.Count);
            for (int i = 0; i < Instance.mEntities.Count; i++)
            {
                writer.Write(Instance.mEntities[i]);
            }

            writer.Write(Instance.mActiveEntities.Count);
            for (int i = 0; i < Instance.mActiveEntities.Count; i++)
            {
                writer.Write(Instance.mActiveEntities[i]);
            }

            writer.Write(Instance.mLastFreeIndex);
        }

        /// <summary>
        /// Read entity store
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public static void Read(BinaryReader reader)
        {
            int capacity = reader.ReadInt32();
            Instance.Initialize(capacity);

            for (int i = 0; i < capacity; i++)
            {
                Instance.mEntityIDs[i].Read(reader);
            }

            capacity = reader.ReadInt32();
            for (int i = 0; i < capacity; i++)
            {
                Instance.mFreeIndecies[i] = reader.ReadInt32();
            }

            capacity = reader.ReadInt32();
            for (int i = 0; i < capacity; i++)
            {
                Instance.mEntities[i] = reader.ReadEntity();
            }

            capacity = reader.ReadInt32();
            for (int i = 0; i < capacity; i++)
            {
                EntityID eid = new EntityID();
                eid.Read(reader);
                Instance.mActiveEntities.Add(eid);
            }

            Instance.mLastFreeIndex = reader.ReadInt32();
        }

        /// <summary>
        /// Initialize entity store, with given capacity
        /// </summary>
        /// <param name="initialCapacity">Capacity</param>
        private void Initialize(int initialCapacity)
        {
            mActiveEntities.Clear();
            mEntities.Clear();
            mEntityIDs.Clear();
            mFreeIndecies.Clear();

            mEntities.Capacity = initialCapacity;
            mEntityIDs.Capacity = initialCapacity;
            mFreeIndecies.Capacity = initialCapacity;

            for (int i = 0; i < initialCapacity; i++)
            {
                mFreeIndecies.Add(i);
                mLastFreeIndex = i;
                mEntityIDs.Add(new EntityID(i, 0));
                mEntities.Add(null);
            }

            mCapacity = initialCapacity;

            // Create a dummy null entity that we can return from Get()
            // and skip one conditional statement (still have to test Gen)
            mEntityIDs[mFreeIndecies[0]].NextGen();
            EntityID id = mEntityIDs[mFreeIndecies[0]];

            // Remove the free index by swapping with last free one
            mFreeIndecies[0] = mFreeIndecies[mLastFreeIndex];
            mLastFreeIndex--;

            mEntities[id.index] = null;
        }
    }
}
