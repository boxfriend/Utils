using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boxfriend.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Recursively changes game object and its children to specified layer
        /// </summary>
        /// <param name="layer">Layer to change all objects to</param>
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform) 
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }
		
        /// <summary>
        /// Returns the first child of the current GameObject that has the specified tag. Does not include itself.
        /// </summary>
		public static GameObject FindChildWithTag(this GameObject obj, string tag)
		{
			foreach(Transform child in obj.transform)
			{
				if(child.gameObject == obj) continue;
				
				if(child.CompareTag(tag))
					return child.gameObject;
			}
			
			return null;
		}
		
        /// <summary>
        /// Returns an array containing all children of the current GameObject that have the specified tag. Does not include itself.
        /// </summary>
		public static GameObject[] FindChildrenWithTag(this GameObject obj, string tag)
		{
			var taggedArray = new GameObject[obj.transform.childCount];
			var index = 0;
			foreach(Transform child in obj.transform)
			{
				if(child.CompareTag(tag))
				{
					taggedArray[index] = child.gameObject;
					index++;
				}
			}
			
			if(index == 0) return null;

			Array.Resize(ref taggedArray, index);
			return taggedArray;
		}
		
        /// <summary>
        /// Returns a List containing all children of the current GameObject that have the specified tag. Does not include itself.
        /// </summary>
		public static List<GameObject> FindChildrenWithTagList(this GameObject obj, string tag)
		{
			var taggedList = new List<GameObject>();
			
			foreach(Transform child in obj.transform)
			{
				if(child.gameObject == obj) continue;
				
				if(child.CompareTag(tag))
					taggedList.Add(child.gameObject);
			}
			return taggedList;
		}

        
        /// <summary>
        /// Destroys all children of the GameObject not including itself\
        /// </summary>
        public static void DestroyChildren(this GameObject parent)
        {
            var children = new Transform[parent.transform.childCount];
            for (var i = 0; i < parent.transform.childCount; i++)
                children[i] = parent.transform.GetChild(i);
            for (var i = 0; i < children.Length; i++)
                GameObject.Destroy(children[i].gameObject);
        }
		
		///<summary>
		/// Checks if a GameObject is tagged with any of the strings in the provided collection
		///</summary>
		public static bool CompareTags(this GameObject go, IEnumerable<string> tags)
		{
			foreach (var tag in tags)
			{
				if (go.CompareTag(tag))
					return true;
			}
			return false;
		}
    }
}
