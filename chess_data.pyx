import pygame

cdef int cstart_x = 34
cdef int cstart_y = 51
cdef int cstart_x2 = 260
cdef int cstart_y2 = 51

SCREEN_SIZE = (521, 313)
pygame.init()

def index_to_chess_select(int index, chess_image_sel):

    if 0 <= index < 5:
        return chess_image_sel[0]
    elif index < 7:
        return chess_image_sel[1]
    elif index < 9:
        return chess_image_sel[2]
    elif index < 11:
        return chess_image_sel[3]
    elif index < 13:
        return chess_image_sel[4]
    elif index < 15:
        return chess_image_sel[5]
    elif 15 == index:
        return chess_image_sel[6]
    elif 16 <= index < 21:
        return chess_image_sel[7]
    elif index < 23:
        return chess_image_sel[8]
    elif index < 25:
        return chess_image_sel[9]
    elif index < 27:
        return chess_image_sel[10]
    elif index < 29:
        return chess_image_sel[11]
    elif index < 31:
        return chess_image_sel[12]
    elif 31 == index:
        return chess_image_sel[13]

def index_to_chess_surface(int index, chess_image):
    
    if 0 <= index < 5:
        return chess_image[0]
    elif index < 7:
        return chess_image[1]
    elif index < 9:
        return chess_image[2]
    elif index < 11:
        return chess_image[3]
    elif index < 13:
        return chess_image[4]
    elif index < 15:
        return chess_image[5]
    elif 15 == index:
        return chess_image[6]
    elif 16 <= index < 21:
        return chess_image[7]
    elif index < 23:
        return chess_image[8]
    elif index < 25:
        return chess_image[9]
    elif index < 27:
        return chess_image[10]
    elif index < 29:
        return chess_image[11]
    elif index < 31:
        return chess_image[12]
    elif 31 == index:
        return chess_image[13]
    elif 32 == index:
        return chess_image[14]
        
cdef int index_to_chess_value(int index):
    if 0 <= index < 5:
        return 1
    elif index < 7:
        return 2
    elif index < 9:
        return 3
    elif index < 11:
        return 4
    elif index < 13:
        return 5
    elif index < 15:
        return 6
    elif 15 == index:
        return 7
    elif 16 <= index < 21:
        return 1
    elif index < 23:
        return 2
    elif index < 25:
        return 3
    elif index < 27:
        return 4
    elif index < 29:
        return 5
    elif index < 31:
        return 6
    elif 31 == index:
        return 7
        
cdef int index_to_color(int index):
    if 0 <= index < 16:
        return 0
    else:
        return 1
        
def color_value_to_index(color, int value, bvalue_num):
    if 0 == bvalue_num[color][value]:
        return None
    
    if 0 == color:
        if 1 == value:
            return bvalue_num[color][value] - 1
        elif 2 == value:
            return bvalue_num[color][value] + 4
        elif 3 == value:
            return bvalue_num[color][value] + 6
        elif 4 == value:
            return bvalue_num[color][value] + 8
        elif 5 == value:
            return bvalue_num[color][value] + 10
        elif 6 == value:
            return bvalue_num[color][value] + 12
        elif 7 == value:
            return bvalue_num[color][value] + 14
    elif 1 == color:
        if 1 == value:
            return bvalue_num[color][value] + 15
        elif 2 == value:
            return bvalue_num[color][value] + 20
        elif 3 == value:
            return bvalue_num[color][value] + 22
        elif 4 == value:
            return bvalue_num[color][value] + 24
        elif 5 == value:
            return bvalue_num[color][value] + 26
        elif 6 == value:
            return bvalue_num[color][value] + 28
        elif 7 == value:
            return bvalue_num[color][value] + 30