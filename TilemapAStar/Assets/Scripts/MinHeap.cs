// Editted from generic heap: https://gist.github.com/roufamatic/ee7e11469809f2b276c0d3dc6b8dd80b
// I modified it into a min heap, added comments, and a added a method to speficially update a chosen element from the outside, to consider a lower inserted value
//      - Contains(T) returns true is the heap has item
//      - UpdateUp(T)
using System;
public class MinHeap<T> where T : IComparable<T>
{
    T[] heap;
    int count;
    public MinHeap(int minSize){
        heap = new T[(int) Math.Abs(minSize) + 1];//((int)Math.Pow(2, Math.Ceiling(Math.Log(minSize, 2))))];//figure out why this is a mess
    }
    public int Count { get { return count; } }
    public bool IsEmpty { get { return count == 0; } }
    public void Insert(T val){//insert an element at the end of the heap and then shift it up
        if (count == heap.Length){//if the heap isnt big enough
            DoubleSize();//double its size
        }
        heap[count] = val;//put element at the end of the heap
        ShiftUp(count);//shift the element to where it should be
        count++;
    }
    public T Peek(){//check the min element
        if (heap.Length == 0) throw new ArgumentOutOfRangeException("No values in heap");
        return heap[0];
    }
    public T Remove(){//remove and return min element
        T output = Peek();
        count--;//make last value be ignored and overriden later
        heap[0] = heap[count];//swap the first and last elements 
        ShiftDown(0);//shifting the lowest element from the root down the tree will reorder the tree correctly after removal
        return output;
    }
    public bool Contains(T value){
        return Array.IndexOf(heap, value) > -1; //array search will return -1 if the element is not there
    }
    public void UpdateUp(T value){//when you update the value and it went down, this will move it up the heap if needed
        int index = Array.IndexOf(heap, value);
        heap[index] = value;
        ShiftUp(index);
    }
    private void ShiftUp(int heapIndex){//move an element up in the heap recursively until its correctly placed
        if (heapIndex == 0) 
            return;//cant shift up past the root
        int parentIndex = (heapIndex - 1) / 2;
        bool shouldShift = heap[parentIndex].CompareTo(heap[heapIndex]) > 0;//shift up if parent is larger
        if (!shouldShift) 
            return;
        Swap(parentIndex, heapIndex);
        ShiftUp(parentIndex);//recursive call from the new location
    }
    private void ShiftDown(int heapIndex){//move an element down in the heap recursively until its correctly placed
        //find both the children
        int child1 = heapIndex * 2 + 1;
        if (child1 >= count) //if there are no children, theres nowhere else to go
            return;
        int child2 = child1 + 1;

        int preferredChildIndex = (child2 >= count || heap[child1].CompareTo(heap[child2]) <= 0) ? child1 : child2;//pick the smaller child, and pick child 1 if its the only one
        if (heap[preferredChildIndex].CompareTo(heap[heapIndex]) > 0) 
            return;//if this parent is the smallest, its in the righ place
        Swap(heapIndex, preferredChildIndex);//if the parent is larger swap it down
        ShiftDown(preferredChildIndex);//recursively call to the new location
    }
    private void Swap(int index1, int index2){//swap by arrary index
        T temp = heap[index1];//need to store one so you can swap without overriding
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }
    private void DoubleSize(){//double the size of the underlieing array
        var copy = new T[heap.Length * 2];
        for (int i = 0; i < heap.Length; i++){//indivually copy each value over
            copy[i] = heap[i];
        }
        heap = copy;
    }
}
