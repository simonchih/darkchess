import math
from chess_data import *
from chess_data cimport *

cdef double drag = 0.999

class chess():
    def __init__(self, int index, rc):
        (row, col) = rc
        self.row = row
        self.col = col
        self.size = chess_back.get_size()
        self.index = index
        # To avoid deepcopy error with Python 3.6
        #self.surface = index_to_chess_surface(index)
        #self.select = index_to_chess_select(index)
        self.color = index_to_color(index)
        self.value = index_to_chess_value(index)
        if col < 4:
            self.x = cstart_x+col*chess_back.get_width()
            self.y = cstart_y+row*chess_back.get_height()
        else:
            self.x = cstart_x2+(col-4)*chess_back.get_width()
            self.y = cstart_y2+row*chess_back.get_height()
        self.back = 1
        self.live = 1
        self.speed = 0
        self.angle = 0
        self.possible_move = []
        
    def draw(self, screen):
        global chess_back
        
        surface = index_to_chess_surface(self.index)
        select = index_to_chess_select(self.index)
        
        if 1 == self.live:
            if 1 == self.back:
                screen.blit(chess_back, (self.x, self.y))
            elif -1 == self.back:
                screen.blit(select, (self.x, self.y))
            else:
                screen.blit(surface, (self.x, self.y))
                
    def click(self, mXY):
        (mouseX, mouseY) = mXY
        if 1 == self.live:
            if self.x < mouseX < self.x + self.size[0] and self.y < mouseY < self.y + self.size[1]:
                if 1 == self.back:
                    #self.back = 0
                    return self.index
                else:
                    return -1
        return None
                    
    def move(self):
        self.x += math.sin(self.angle) * self.speed
        self.y -= math.cos(self.angle) * self.speed
        self.speed *= drag