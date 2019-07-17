# Dynamo Space Partitioning

A Dynamo package for common spatial search and intersection tests on large amounts of objects using tree data structures. The implementations here are currently naive and unoptimized, but they should still be considerably faster than brute force strategies.

### Currently supported:

* Octrees
   * Hit tests (Point intersection)
   * Range queries (Bounding box intersection)
   * Radius queries (Sphere intersection)

### Future possibilities:

* Octrees
   * Nearest neighbor search
   * Ray intersection
   * Visibility tests (Frustum intersection)
* K-D trees
   * Same complement of queries with different performance characteristics
* 2D optimized versions of Octrees and K-D trees
* Tuning of tree parameters for advanced users
* Fancier implementations (skip octrees, etc.)

## In Use
*Examples/Octree Simple Example.dyn*

![Screenshot of Octree Simple Example.dyn graph in Examples folder](Examples/SimpleExample.PNG)

