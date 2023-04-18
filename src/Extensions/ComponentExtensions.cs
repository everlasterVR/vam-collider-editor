using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColliderEditor.Extensions
{
    public static class ComponentExtensions
    {
        public static string Uuid(this Component component)
        {
            var paths = BuildStack(component);
            return component.GetTypeName() + ":" + string.Join(".", paths.ToArray());
        }

        static Stack<string> BuildStack(Component component)
        {
            var paths = new Stack<string>(new[] { $"{component.name}" });
            var current = component.gameObject.transform;

            while(CanNavigateUp(current))
            {
                paths.Push(current.name);
                current = current.transform.parent;
            }

            return paths;
        }

        static bool CanNavigateUp(Transform current)
        {
            if(current == null)
            {
                return false;
            }

            if(current.name.Equals("geometry", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if(current.name.Equals("Genesis2Female", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return !current.name.Equals("Genesis2Male", StringComparison.InvariantCultureIgnoreCase);

        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string GetTypeName(this Component component)
        {
            if(component is CapsuleCollider)
            {
                return nameof(CapsuleCollider);
            }

            if(component is SphereCollider)
            {
                return nameof(SphereCollider);
            }

            if(component is BoxCollider)
            {
                return nameof(BoxCollider);
            }

            if(component is AutoCollider)
            {
                return nameof(AutoCollider);
            }

            if(component is AutoColliderGroup)
            {
                return nameof(AutoColliderGroup);
            }

            if(component is Rigidbody)
            {
                return nameof(Rigidbody);
            }

            throw new InvalidOperationException($"Unknown component type: {component.GetType()}");
        }
    }
}
