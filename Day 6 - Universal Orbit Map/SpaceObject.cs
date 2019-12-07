using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalOrbitMap
{
    public class SpaceObject
    {
        public static SpaceObject MapOrbits(string[] orbits)
        {
            var COM = new SpaceObject("COM", null);
            var orbitDefs = orbits.AsParallel().Select(o => MapDefinition(o));
            COM.TraverseDefinitions(orbitDefs.ToList());
            return COM;
        }

        public void TraverseDefinitions(List<OrbitDefinition> orbitDefinitions)
        {
            var definitionsForMe = orbitDefinitions.Where(o => o.ParentID == this.Id);
            foreach(OrbitDefinition def in definitionsForMe)
            {
                var SpaceObject = new SpaceObject(def.ChildID, this);
                SpaceObject.TraverseDefinitions(orbitDefinitions);
                Orbiters.Add(SpaceObject);
            }
        }

        private static OrbitDefinition MapDefinition(string definition)
        {
            var orbitDefinition = new OrbitDefinition();
            var pairs = definition.Split(')');
            orbitDefinition.ParentID = pairs[0];
            orbitDefinition.ChildID = pairs[1];
            return orbitDefinition;
        }

        public SpaceObject(string id, SpaceObject parent)
        {
            this.Id = id;
            this.Parent = parent;
        }

        public SpaceObject Find(string id)
        {
            if (this.Id == id)
                return this;

            foreach(var obj in Orbiters){
                var toFind = obj.Find(id);
                if (toFind != null)
                    return toFind;
            }
            return null;
        }

        public int FindShortestStepsTo(string id, int depth)
        {
            if (this.Id == id)
                return depth;

            foreach (var obj in Orbiters)
            {
                var finalDepth = obj.FindShortestStepsTo(id, depth + 1);
                if (finalDepth != -1)
                    return finalDepth;
            }
            return -1;
        }

        public int FindShortestStepsToUp(string id, string lastChecked, int depth)
        {
            if (this.Id == id)
                return depth;

            if (this.Orbiters.Count <= 1)
                return Parent.FindShortestStepsToUp(id, this.Id, depth + 1);

            int shortestSteps = -1;
            foreach (var obj in Orbiters)
            {
                if (obj.Id == lastChecked)
                    continue;
                var toFind = obj.FindShortestStepsTo(id, depth + 1);
                if (toFind == -1)
                    continue;
                if(shortestSteps == -1)
                {
                    shortestSteps = toFind;
                    continue;
                }
                else if (shortestSteps > toFind)
                {
                    shortestSteps = toFind;
                }
            }
            if (shortestSteps == -1)
                return Parent.FindShortestStepsToUp(id, this.Id, depth + 1);
            return shortestSteps;
        }

        public SpaceObject FindUp(string id, string lastChecked)
        {
            if (this.Id == id)
                return this;

            if (this.Orbiters.Count <= 1)
                return Parent.FindUp(id, this.Id);

            foreach(var obj in Orbiters)
            {
                if (obj.Id == lastChecked)
                    continue;
                var toFind = obj.Find(id);
                if (toFind != null)
                    return toFind;
            }

            return Parent?.FindUp(id, this.Id);
        }

        public int CountOrbits(int depth)
        {
            var orbits = depth;
            foreach(var obj in Orbiters)
            {
                orbits += obj.CountOrbits(depth + 1);
            }
            return orbits;
        }

        public int ShortestStepsFromParentToParentOf(string id)
        {
            var steps = FindShortestStepsTo(id, -1);
            if (steps > 0)
                return steps - 1;
            steps = FindShortestStepsToUp(id, this.Id, -1);
            if (steps > 0)
                return steps - 1;
            return -1;
        }

        public SpaceObject Parent { get; set; }
        public string Id { get; set; }
        public List<SpaceObject> Orbiters { get; set; } = new List<SpaceObject>();
    }

    public class OrbitDefinition
    {
        public string ParentID { get; set; }
        public string ChildID { get; set; }
    }
}
