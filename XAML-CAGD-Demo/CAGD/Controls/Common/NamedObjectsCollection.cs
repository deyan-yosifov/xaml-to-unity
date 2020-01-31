using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CAGD.Controls.Common
{
    public class NamedObjectsCollection<T> : IEnumerable, IEnumerable<T>
        where T : INamedObject
    {
        private readonly List<T> elements;

        public NamedObjectsCollection()
        {
            this.elements = new List<T>();
        }

        public INamedObject this[string name]
        {
            get
            {
                return this.elements[this.GetExistingElementIndex(name)];
            }
        }

        public void AddLast(T element)
        {
            this.EnsureUniqueName(element);
            this.elements.Add(element);
        }

        public void AddFirst(T element)
        {
            this.EnsureUniqueName(element);
            this.elements.Insert(0, element);
        }

        public void InsertBefore(T elementToInsert, string existingElementName)
        {
            this.InsertRelativeTo(elementToInsert, 0, existingElementName);
        }

        public void InsertAfter(T elementToInsert, string existingElementName)
        {
            this.InsertRelativeTo(elementToInsert, 1, existingElementName);
        }

        public bool TryGetElementOfType<W>(string name, out W element)
            where W : class, T
        {
            int index;
            if (this.TryGetElementIndex(name, out index))
            {
                element = this.elements[index] as W;
                if (element != null)
                {
                    return true;
                }
            }
            
            element = null;
            return false;
        }

        private void InsertRelativeTo(T elementToInsert, int relativeIndexDistance, string existingElementName)
        {
            int index = this.GetExistingElementIndex(existingElementName);
            this.EnsureUniqueName(elementToInsert);
            this.elements.Insert(index + relativeIndexDistance, elementToInsert);
        }

        private int GetExistingElementIndex(string alreadyAddedName)
        {
            int index;
            if (this.TryGetElementIndex(alreadyAddedName, out index))
            {
                return index;
            }

            throw new InvalidOperationException(string.Format("Cannot find element with the specified name: {0}", alreadyAddedName));
        }

        private bool TryGetElementIndex(string name, out int elementIndex)
        {
            for (int index = 0; index < this.elements.Count; index++)
            {
                if (this.elements[index].Name.Equals(name))
                {
                    elementIndex = index;
                    return true;
                }
            }

            elementIndex = -1;
            return false;
        }

        private void EnsureUniqueName(INamedObject newlyAddedElement)
        {
            if (this.elements.Any((existingElement) => existingElement.Name.Equals(newlyAddedElement.Name)))
            {
                throw new InvalidOperationException(string.Format("Cannot add element with the same name: {0}", newlyAddedElement.Name));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T element in this.elements)
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T element in this.elements)
            {
                yield return element;
            }
        }
    }
}
