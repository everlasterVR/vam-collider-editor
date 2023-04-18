using ColliderEditor.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColliderEditor.Models
{
    abstract class ModelBase<T> where T : Component
    {
        bool _highlighted;
        protected readonly T component;
        public List<Group> Groups { get; set; }
        public ModelBase<T> Mirror { get; set; }

        public IModel MirrorModel
        {
            get { return Mirror as IModel; }
        }

        public string Id { get; }
        public string Label { get; }
        public bool Selected { get; set; }

        protected ModelBase(T component, string label)
        {
            if(component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if(string.IsNullOrEmpty(label))
            {
                throw new ArgumentException("message", nameof(label));
            }

            this.component = component;
            Groups = new List<Group>();
            Id = component.Uuid();
            Label = label;
        }

        public bool Highlighted
        {
            get { return _highlighted; }
            set
            {
                if(_highlighted != value)
                {
                    SetHighlighted(value);
                    _highlighted = value;
                }
            }
        }

        protected virtual void SetHighlighted(bool value)
        {
        }

        public abstract void SyncPreviews();

        public override string ToString()
        {
            return Id;
        }
    }
}
