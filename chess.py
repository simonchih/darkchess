import math
from chess_data import *

drag = 0.999

class chess():
    def __init__(self, index, (x, y), (row, col), size):
        self.x = x
        self.y = y
        self.row = row
        self.col = col
        self.size = size
        self.index = index
        self.surface = index_to_chess_surface(index)
        self.select = index_to_chess_select(index)
        self.color = index_to_color(index)
        self.value = index_to_chess_value(index)
        self.back = 1
        self.live = 1
        self.speed = 0
        self.angle = 0
        self.possible_move = []
        
    def draw(self, screen, chess_back):
        if 1 == self.live:
            if 1 == self.back:
                screen.blit(chess_back, (self.x, self.y))
            elif -1 == self.back:
                screen.blit(self.select, (self.x, self.y))
            else:
                screen.blit(self.surface, (self.x, self.y))
                
    def click(self, (mouseX, mouseY)):
        if 1 == self.live:
            if self.x < mouseX < self.x + self.size[0] and self.y < mouseY < self.y + self.size[1]:
                if 1 == self.back:
                    self.back = 0
                    return self.index
                else:
                    return -1
        return None
                    
    def move(self):
        self.x += math.sin(self.angle) * self.speed
        self.y -= math.cos(self.angle) * self.speed
        self.speed *= drag