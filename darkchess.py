import random, os
import math
import time 
import pygame
from pygame.locals import *
from sys import exit
from chess import *

background_image_filename = 'Image/SHEET.gif'
image_chess_back = 'Image/back.gif'
image_chess_bk = 'Image/BK.gif'
image_chess_ba = 'Image/BA.gif'
image_chess_bb = 'Image/BB.gif'
image_chess_br = 'Image/BR.gif'
image_chess_bn = 'Image/BN.gif'
image_chess_bc = 'Image/BC.gif'
image_chess_bp = 'Image/BP.gif'
image_chess_rk = 'Image/RK.gif'
image_chess_ra = 'Image/RA.gif'
image_chess_rb = 'Image/RB.gif'
image_chess_rr = 'Image/RR.gif'
image_chess_rn = 'Image/RN.gif'
image_chess_rc = 'Image/RC.gif'
image_chess_rp = 'Image/RP.gif'

SCREEN_SIZE = (521, 313) 
pygame.init()

screen = pygame.display.set_mode(SCREEN_SIZE, 0, 32)#SCREEN_SIZE, FULLSCREEN, 32)
pygame.display.set_caption("Taiwan Blind Chess")

background = pygame.image.load(background_image_filename).convert()
chess_back = pygame.image.load(image_chess_back).convert()
chess_bk = pygame.image.load(image_chess_bk).convert()
chess_ba = pygame.image.load(image_chess_ba).convert()
chess_bb = pygame.image.load(image_chess_bb).convert()
chess_br = pygame.image.load(image_chess_br).convert()
chess_bn = pygame.image.load(image_chess_bn).convert()
chess_bc = pygame.image.load(image_chess_bc).convert()
chess_bp = pygame.image.load(image_chess_bp).convert()
chess_rk = pygame.image.load(image_chess_rk).convert()
chess_ra = pygame.image.load(image_chess_ra).convert()
chess_rb = pygame.image.load(image_chess_rb).convert()
chess_rr = pygame.image.load(image_chess_rr).convert()
chess_rn = pygame.image.load(image_chess_rn).convert()
chess_rc = pygame.image.load(image_chess_rc).convert()
chess_rp = pygame.image.load(image_chess_rp).convert()

cstart_x = 34
cstart_y = 51
cstart_x2 = 260
cstart_y2 = 51

player_first = 0
first = 1
turn_id = 0
player_color = 0
com_color = 0

chtemp = chess(0, 0, 0, (0, 0), (0, 0), chess_back.get_size(), chess_back)
my_ch = [[chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp]]
chess_index = [0] * 32
map = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
cor = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]

def index_to_chess_surface(index):
    if 0 <= index < 5:
        return chess_bp
    elif index < 7:
        return chess_bc
    elif index < 9:
        return chess_bn
    elif index < 11:
        return chess_br
    elif index < 13:
        return chess_bb
    elif index < 15:
        return chess_ba
    elif 15 == index:
        return chess_bk
    elif 16 <= index < 21:
        return chess_rp
    elif index < 23:
        return chess_rc
    elif index < 25:
        return chess_rn
    elif index < 27:
        return chess_rr
    elif index < 29:
        return chess_rb
    elif index < 31:
        return chess_ra
    elif 31 == index:
        return chess_rk

def index_to_chess_value(index):
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

def ini_random_chess(list):
    all_list = [0] * 32
    for end in range(31, -1, -1):
        start = random.randint(0, end)
        i = start
        while i != -1 :
            if 0 == all_list[start]:
                if i != 0:
                    start += 1
                    start %= 32
                    i -= 1
                else:
                    break
            else:
                start += 1
                start %= 32
        all_list[start] = 1
        list[31-end] = start
    return list

def index_to_color(index):
    if 0 <= index < 16:
        return 0
    else:
        return 1

def findC(ch, x, y):
    for pr in ch:
        for p in pr:
            if math.hypot(p.x-x, p.y-y) <= p.size:
                return p
    return None

def all_chess_move(map, my_chess):
    for rowm in map:
        for m in rowm:
            if m!= (-1, -1) and 0 == my_chess[m[0]][m[1]].back:
                my_chess[m[0]][m[1]].possible_move = collect_possible_move(my_chess[m[0]][m[1]].row, my_chess[m[0]][m[1]].col) 
    
def collect_possible_move(i, j):
    global map
    global my_ch
    
    pm = []
    ncor = near(i,j)
    for nc in ncor:
        if (-1, -1) == map[nc[0]][nc[1]]:
            pm.append(nc)
        elif my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].color != my_ch[map[i][j][0]][map[i][j][1]].color and 0 == my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].back:
            if 1 == my_ch[map[i][j][0]][map[i][j][1]].value and 7 == my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].value:
                pm.append(nc)
            elif 7 == my_ch[map[i][j][0]][map[i][j][1]].value and 1 == my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].value:
                pass
            elif my_ch[map[i][j][0]][map[i][j][1]].value != 2 and my_ch[map[i][j][0]][map[i][j][1]].value >= my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].value:
                pm.append(nc)
    if 2 == my_ch[map[i][j][0]][map[i][j][1]].value:
        jump = 0
        for ii in range(i-1, -1, -1):
            if 1 == jump:
                if map[ii][j] == (-1, -1):
                    pass
                elif 1 == my_ch[map[ii][j][0]][map[ii][j][1]].back or my_ch[map[ii][j][0]][map[ii][j][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((ii, j))
                    break
            if map[ii][j] != (-1, -1):
                jump = 1
        jump = 0
        for ii in range(i+1, 4, 1):
            if 1 == jump:
                if map[ii][j] == (-1, -1):
                    pass
                elif 1 == my_ch[map[ii][j][0]][map[ii][j][1]].back or my_ch[map[ii][j][0]][map[ii][j][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((ii, j))
                    break
            if map[ii][j] != (-1, -1):
                jump = 1
        jump = 0
        for jj in range(j-1, -1, -1):
            if 1 == jump:
                if map[i][jj] == (-1, -1):
                    pass
                elif 1 == my_ch[map[i][jj][0]][map[i][jj][1]].back or my_ch[map[i][jj][0]][map[i][jj][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((i, jj))
                    break
            if map[i][jj] != (-1, -1):
                jump = 1
        jump = 0
        for jj in range(j+1, 8, 1):
            if map[i][jj] == (-1, -1):
                    pass
            elif 1 == jump:
                if 1 == my_ch[map[i][jj][0]][map[i][jj][1]].back or my_ch[map[i][jj][0]][map[i][jj][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((i, jj))
                    break
            if map[i][jj] != (-1, -1):
                jump = 1
    return pm
    
def near(i, j):
    n_cor = []
    if 0 == i and 0 == j:
        n_cor.extend([(1,0), (0,1)])
    elif 3 == i and 0 == j:
        n_cor.extend([(2, 0), (3, 1)])
    elif 0 == i and 7 == j:
        n_cor.extend([(0, 6), (1, 7)])
    elif 3 == i and 7 == j:
        n_cor.extend([(3, 6), (2, 7)])
    elif 0 == j:
        n_cor.extend([(i-1, j), (i+1, j), (i, j+1)])
    elif 0 == i:
        n_cor.extend([(i, j-1), (i, j+1), (i+1, j)])
    elif 7 == j:
        n_cor.extend([(i-1, j), (i+1, j), (i, j-1)])
    elif 3 == i:
        n_cor.extend([(i, j-1), (i, j+1), (i-1, j)])
    else:
        n_cor.extend([(i-1, j), (i+1, j), (i, j+1), (i, j-1)])
    
    return n_cor
    
def mouse_position_to_block(mx, my, chess_back):
    global cstart_x
    global cstart_y
    global cstart_x2
    global cstart_y2
    
    for i in range(0, 4):
        for j in range(0, 4):
            if cstart_x+j*chess_back.get_width() < mx < cstart_x+(j+1)*chess_back.get_width() and cstart_y+i*chess_back.get_height() < my < cstart_y+(i+1)*chess_back.get_height():
                return (i, j)
        
    for i in range(0, 4):
        for j in range(0, 4):
            if cstart_x2+j*chess_back.get_width() < mx < cstart_x2+(j+1)*chess_back.get_width() and cstart_y2+i*chess_back.get_height() < my < cstart_y2+(i+1)*chess_back.get_height():
                return (i, 4+j)

def chess_ai():
    global turn_id
    global first
    global my_ch
    global player_color
    global com_color
    global player_first
    
    if 0 == player_first and  1 == first:
        i = random.randint(0, 3) 
        j = random.randint(0, 7) 
        turn_id = my_ch[i][j].color
        my_ch[i][j].back = 0
        com_color = turn_id
        player_color = 1 - com_color
        first = 0
    if turn_id == com_color:
        turn_id = 1 - turn_id
                
def main():
    global cstart_x
    global cstart_y
    global cstart_x2
    global cstart_y2
    global chess_index
    global turn_id
    global player_color
    global com_color
    global player_first
    global my_ch
    global map
    global cor
    global first
    
    selected_c = None
    first = 1
    move = 1
    
    player_first = random.randint(0, 1)
    
    chess_index = ini_random_chess(chess_index)
    for i in range(0, 4):
        for j in range(0, 4):
            ch = chess(chess_index[8*i+j],index_to_color(chess_index[8*i+j]), index_to_chess_value(chess_index[8*i+j]), (cstart_x+j*chess_back.get_width(),cstart_y+i*chess_back.get_height()), (i, j), chess_back.get_size(), index_to_chess_surface(chess_index[8*i+j]))
            my_ch[i][j] = ch
            cor[i][j] = (ch.x, ch.y)
            map[i][j] = (i, j)
    for i in range(0, 4):
        for j in range(0, 4):
            ch = chess(chess_index[8*i+4+j],index_to_color(chess_index[8*i+4+j]), index_to_chess_value(chess_index[8*i+4+j]), (cstart_x2+j*chess_back.get_width(),cstart_y2+i*chess_back.get_height()), (i, 4+j), chess_back.get_size(), index_to_chess_surface(chess_index[8*i+4+j]))
            my_ch[i][4+j] = ch
            cor[i][4+j] = (ch.x, ch.y)
            map[i][4+j] = (i, 4+j)
    
    while True:
        screen.blit(background, (0,0))
        
        if 1 == move:
            all_chess_move(map, my_ch)
            move = 0
        
        for event in pygame.event.get():
            if event.type == QUIT:
                exit()
            elif event.type == pygame.MOUSEBUTTONDOWN:
                for chr in my_ch:
                    for chc in chr:
                        ch_index = chc.click(pygame.mouse.get_pos())
                        if 1 == player_first and 1 == first:
                            turn_id = index_to_color(ch_index)
                            player_color = turn_id
                            com_color = 1 - player_color
                            first = 0
                        elif -1 == ch_index and player_color == turn_id and chc.color == player_color:
                            selected_c = chc
            elif event.type == pygame.MOUSEBUTTONUP:
                if selected_c:
                    (mouseX, mouseY) = pygame.mouse.get_pos()
                    move = 0
                    for pm in selected_c.possible_move:
                        if pm == mouse_position_to_block(mouseX, mouseY, chess_back):
                            if map[pm[0]][pm[1]] != (-1, -1):
                                my_ch[map[pm[0]][pm[1]][0]][map[pm[0]][pm[1]][1]].live = 0
                            map[pm[0]][pm[1]] = map[selected_c.row][selected_c.col]
                            map[selected_c.row][selected_c.col] = (-1, -1)
                            selected_c.x = cor[pm[0]][pm[1]][0]
                            selected_c.y = cor[pm[0]][pm[1]][1]
                            selected_c.row = pm[0]
                            selected_c.col = pm[1]
                            move = 1
                            break
                    
                    if 0 == move:
                       (selected_c.x, selected_c.y) = cor[selected_c.row][selected_c.col] 
                
                    selected_c.speed = 0
                    selected_c = None
                else:
                    move = 1
        
        chess_ai()
        
        if selected_c:
            (mouseX, mouseY) = pygame.mouse.get_pos()
            dx = mouseX - selected_c.x
            dy = mouseY - selected_c.y
            dx -= selected_c.size[0]/2
            dy -= selected_c.size[1]/2
            selected_c.angle = 0.5*math.pi + math.atan2(dy, dx)
            selected_c.speed = math.hypot(dx, dy) * 0.1
        
        
        for cr in my_ch:
            for c in cr:
                c.draw(screen, chess_back)
        
        if selected_c:
            selected_c.move()
            selected_c.draw(screen, chess_back)
        
        pygame.display.update()

if __name__ == "__main__":
    main()