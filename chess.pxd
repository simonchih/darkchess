cdef double drag = 0.999

cdef class chess:
    cdef public int row, col
    cdef public int size[2]
    cdef public int color
    cdef public int index, value, x, y, back, live
    cdef public double speed, angle
    cdef public list possible_move
    
