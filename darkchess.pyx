﻿# Allow pygame_sdl2 to be imported as pygame.
try:
    import pygame_sdl2
    pygame_sdl2.import_as_pygame()
except ImportError:
    pass

import random, os

os.environ['PYGAME_HIDE_SUPPORT_PROMPT'] = "1"

import math
import time 
import pygame
import copy
import threading
from pygame.locals import *
from sys import exit
from multiprocessing import Process, Queue

from chess cimport *
from chess_data import *
from chess_data cimport *

cdef char* background_image_filename = 'Image/SHEET.gif'
cdef char* image_new        = 'Image/shield-and-swords.gif'

cdef char* image_chess_back = 'Image/back.gif'

cdef char* image_chess_bk = 'Image/BK.GIF'
cdef char* image_chess_ba = 'Image/BA.GIF'
cdef char* image_chess_bb = 'Image/BB.GIF'
cdef char* image_chess_br = 'Image/BR.GIF'
cdef char* image_chess_bn = 'Image/BN.GIF'
cdef char* image_chess_bc = 'Image/BC.GIF'
cdef char* image_chess_bp = 'Image/BP.GIF'
cdef char* image_chess_rk = 'Image/RK.GIF'
cdef char* image_chess_ra = 'Image/RA.GIF'
cdef char* image_chess_rb = 'Image/RB.GIF'
cdef char* image_chess_rr = 'Image/RR.GIF'
cdef char* image_chess_rn = 'Image/RN.GIF'
cdef char* image_chess_rc = 'Image/RC.GIF'
cdef char* image_chess_rp = 'Image/RP.GIF'

cdef char* image_chess_bks = 'Image/BKS.GIF'
cdef char* image_chess_bas = 'Image/BAS.GIF'
cdef char* image_chess_bbs = 'Image/BBS.GIF'
cdef char* image_chess_brs = 'Image/BRS.GIF'
cdef char* image_chess_bns = 'Image/BNS.GIF'
cdef char* image_chess_bcs = 'Image/BCS.GIF'
cdef char* image_chess_bps = 'Image/BPS.GIF'
cdef char* image_chess_rks = 'Image/RKS.GIF'
cdef char* image_chess_ras = 'Image/RAS.GIF'
cdef char* image_chess_rbs = 'Image/RBS.GIF'
cdef char* image_chess_rrs = 'Image/RRS.GIF'
cdef char* image_chess_rns = 'Image/RNS.GIF'
cdef char* image_chess_rcs = 'Image/RCS.GIF'
cdef char* image_chess_rps = 'Image/RPS.GIF'

cdef char* s_newgame = 'Sound/NEWGAME.WAV'
cdef char* s_capture = 'Sound/CAPTURE2.WAV'
cdef char* s_click   = 'Sound/CLICK.WAV'
cdef char* s_loss    = 'Sound/LOSS.WAV'
cdef char* s_move2   = 'Sound/MOVE2.WAV'
cdef char* s_win     = 'Sound/WIN.WAV'

cdef int text_x = 237
cdef int text_y = 16
cdef int new_game_iconi = 440
cdef int new_game_iconj = 13

# 0: ini, 1: human win, -1: com win, -2: drawn game
cdef int player_win = 0
cdef int player_first = 0
cdef int first = 1
# turn_id, 0:black, 1:red, 2:process_moving
cdef int turn_id = 0
cdef int player_color = 0
cdef int com_color = 1
cdef int sindex = 0
cdef double AI_min_score = 9000.0
cdef double final_score = 9000.0 #mini
cdef list gb_m2 = []
#max_cor = None
open_score = None
cdef int step = 0

cdef list main_chess = [[0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0 ,0, 0],  [0, 0, 0, 0, 0, 0 ,0, 0],  [0, 0, 0, 0, 0, 0 ,0, 0]]

cdef list server_main_chess = [[0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0 ,0, 0],  [0, 0, 0, 0, 0, 0 ,0, 0],  [0, 0, 0, 0, 0, 0 ,0, 0]]

cdef int chess_index[32]
chess_index[:] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

cdef list main_map = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
cdef list cor = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]

# back_value_num[0, 1] color, index 0: reserved, index 1~7, the back chess number of chess value. (1-based)
# e.g. back_value_num[0][3] = 2, color=0, chess value 3 have 2 back pieces
cdef int back_value_num[2][8]
back_value_num[0][:] = [0, 0, 0, 0, 0, 0, 0, 0]
back_value_num[1][:] = [0, 0, 0, 0, 0, 0, 0, 0]
#back_value_num = [ [0] * 8, [0] * 8 ]

cdef int king_live[2]
king_live[:] = [1, 1]

cdef int chess_num[2]
chess_num[:] = [16, 16]

cdef list com_mv_map = [0, 0]
cdef int back_num = 32
#com_will_eat_chess = []
#will_eat_escape_chess = []
cdef list break_long_capture_dest = []
cdef list break_long_capture_org = []
cdef list com_ban_step = []
cdef list move_step = [None, None, None, None]

max_value = 0.0
max_dist = 32
mark = [[0]*8, [0]*8, [0]*8, [0]*8]
cannon_mark = [[0]*8, [0]*8, [0]*8, [0]*8]

# 20201219 added
pygame.init()

# return 0: can't eat, 1: can eat, 2: equal
cdef int can_be_ate_equal(int small_value, int big_value):
    if 2 == big_value:
        return 0
    elif 1 == big_value and 7 == small_value:
        return 1
    elif 7 == big_value and 1 == small_value:
        return 0
    elif big_value >  small_value:
        return 1
    elif big_value == small_value:
        return 2
    else:
        return 0

cdef int can_be_ate(int small_value, int big_value):       
    if 2 == big_value:
        return 0
    elif 1 == big_value and 7 == small_value:
        return 1
    elif 7 == big_value and 1 == small_value:
        return 0
    elif big_value > small_value:
        return 1
    else:
        return 0
    
cdef list ini_random_chess(list list):
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

#def findC(ch, x, y):
#    for pr in ch:
#        for p in pr:
#            if math.hypot(p.x-x, p.y-y) <= p.size:
#                return p
#    return None

cdef void all_chess_move(a_map, my_chess):
    for chr in my_chess:
        for ch in chr:
            if ch.back < 1 and 1 == ch.live:
                ch.possible_move = collect_possible_move(ch.row, ch.col, a_map, my_chess)
    
cdef list collect_possible_move(int i, int j, a_map, my_chess):
    
    cdef list pm = []
    ncor = near(i,j)
    for nc in ncor:
        if None == a_map[nc[0]][nc[1]]:
            pm.append(nc)
        elif a_map[i][j] != None: #a_map[i][j] != None and a_map[nc[0]][nc[1]] != None 
            if my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].color != my_chess[a_map[i][j][0]][a_map[i][j][1]].color and my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].back < 1:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and 7 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                    pm.append(nc)
                elif 7 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and 1 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                    pass
                elif my_chess[a_map[i][j][0]][a_map[i][j][1]].value != 2 and my_chess[a_map[i][j][0]][a_map[i][j][1]].value >= my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                    pm.append(nc)
    if a_map[i][j] != None:
        if 2 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
            jump = 0
            for ii in range(i-1, -1, -1):
                if 1 == jump and a_map[ii][j] != None:
                    if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((ii, j))
                        break
                if a_map[ii][j] != None:
                    jump = 1
            jump = 0
            for ii in range(i+1, 4, 1):
                if 1 == jump and a_map[ii][j] != None:
                    if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((ii, j))
                        break
                if a_map[ii][j] != None:
                    jump = 1
            jump = 0
            for jj in range(j-1, -1, -1):
                if 1 == jump and a_map[i][jj] != None:
                    if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((i, jj))
                        break
                if a_map[i][jj] != None:
                    jump = 1
            jump = 0
            for jj in range(j+1, 8, 1):
                if 1 == jump and a_map[i][jj] != None:
                    if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((i, jj))
                        break
                if a_map[i][jj] != None:
                    jump = 1
    return pm

#cdef int opp_cannon_can_eat((int, int)org, (int, int)dest, my_chess, a_map):
#    (i, j) = org
#    (ii, jj) = dest
#    o_color = my_chess[a_map[i][j][0]][a_map[i][j][1]].color
#    ch1 = None
#    ch2 = None
#    if i == ii:
#        for ki in range(ii+1, 4):
#            if a_map[ki][jj] != None:
#                (mi, mj) = a_map[ki][jj]
#                ch1 = my_chess[mi][mj]
#                break
#        for ki in range(ii-1, -1, -1):
#            if a_map[ki][jj] != None:
#                (mi, mj) = a_map[ki][jj]
#                ch2 = my_chess[mi][mj]
#                break
#        if ch1 != None and ch2 != None:
#            if (ch1.color == o_color and ch2.color == 1 - o_color and ch1.back < 1 and ch2.back < 1 and ch2.value == 2) or (ch1.color == 1 - o_color and ch2.color == o_color and ch1.back < 1 and ch2.back < 1 and ch1.value == 2):
#                return 1        
#    else:       
#        for kj in range(jj+1, 8):
#            if a_map[ii][kj] != None:
#                (mi, mj) = a_map[ii][kj]
#                ch1 = my_chess[mi][mj]
#                break
#        for kj in range(jj-1, -1, -1):
#            if a_map[ii][kj] != None:
#                (mi, mj) = a_map[ii][kj]
#                ch2 = my_chess[mi][mj]
#                break
#        if ch1 != None and ch2 != None:
#            if (ch1.color == o_color and ch2.color == 1 - o_color and ch1.back < 1 and ch2.back < 1 and ch2.value == 2) or (ch1.color == 1 - o_color and ch2.color == o_color and ch1.back < 1 and ch2.back < 1 and ch1.value == 2):
#                return 1
#    return 0                               
    
cdef int eat_by_bomb((int, int)org, a_map, my_chess):
    (i, j) = org
    cdef int jump = 0
    cdef int was_ate = 0
    for ii in range(i-1, -1, -1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].value:
                was_ate = 1
                break
        elif a_map[ii][j] != None:
            jump = 1
    jump = 0
    for ii in range(i+1, 4, 1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].value:
                was_ate = 1
                break
        elif a_map[ii][j] != None:
            jump = 1
    jump = 0
    for jj in range(j-1, -1, -1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].value:
                was_ate = 1
                break
        elif a_map[i][jj] != None:
            jump = 1
    jump = 0
    for jj in range(j+1, 8, 1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].value:
                was_ate = 1
                break
        elif a_map[i][jj] != None:
            jump = 1
    return was_ate

# analyze all back pieces, it's possible for human player, NOT cheating
cdef int check_eat_number(a_map, my_chess, int n_min, int n_max, int no_min, int no_max, int y, int x):
    global com_color
    global player_color
    
    cdef int eat_possible_num = 0
    cdef int was_ate_num = 0
    
    for c, val in enumerate(back_value_num):
        for v, num in enumerate(val):
            if 0 == num or 0 == v:
                continue
            if com_color == c:
                if 2 == v and n_max < 3:
                    if 0 < if_cannon_can_eat((y, x), a_map, my_chess, com_color):
                        eat_possible_num += num
                elif 0 == n_max:
                    continue
                elif 1 == n_max and 7 == v:
                    was_ate_num += num
                elif 7 == n_max and 1 == v:
                    if 7 == n_min or 2 == n_min:
                        # there are bugs for 2 == n_min
                        eat_possible_num += num
                    else:
                        was_ate_num += num
                elif v > n_max:
                    eat_possible_num += num
                else:
                    if v != 1 or n_max != 2 or n_min != 2:
                        was_ate_num += num
            else:
                if 2 == v:
                    if 0 < if_cannon_can_eat((y, x), a_map, my_chess, player_color):
                        was_ate_num += num
                    elif no_max >= 3 and no_max > n_max:
                        eat_possible_num += num
                elif 8 == no_min:
                    continue
                elif 1 == no_min and 7 == v:
                    if 1 == no_max:
                        eat_possible_num += num
                    else:
                        was_ate_num += num
                elif 7 == no_max and 1 == v:
                        was_ate_num += num
                elif v >= no_min:
                    was_ate_num += num
                else:
                    if v != 1 or no_max != 2 or no_min != 2:
                        if no_max > n_max:
                            eat_possible_num += num
    
    return eat_possible_num - was_ate_num

cpdef int if_cannon_can_eat((int, int)org, a_map, my_chess, int owner_color):
    (i, j) = org
    cdef int jump = 0
    cdef int eat_number = 0
    cdef int opp_color = 1 - owner_color
    
    for ii in range(i-1, -1, -1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back:
                break
            elif 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].live and opp_color == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color:
                eat_number += 1
                break
        elif a_map[ii][j] != None:
            jump = 1
    jump = 0
    for ii in range(i+1, 4, 1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back:
                break
            elif 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].live and opp_color == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color:
                eat_number += 1
                break
        elif a_map[ii][j] != None:
            jump = 1
    jump = 0
    for jj in range(j-1, -1, -1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back:
                break
            elif 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].live and opp_color == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color:
                eat_number += 1
                break
        elif a_map[i][jj] != None:
            jump = 1
    jump = 0
    for jj in range(j+1, 8, 1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back:
                break
            elif 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].live and opp_color == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color:
                eat_number += 1
                break
        elif a_map[i][jj] != None:
            jump = 1
    
    return eat_number
   
cpdef int eat_by_player_bomb((int, int)org, a_map, my_chess, int player_color):
    (i, j) = org
    cdef int jump = 0
    cdef int was_ate = 0
    for ii in range(i-1, -1, -1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back:
                break
            elif 2 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].value and player_color == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color:
                was_ate = 1
                break
        elif a_map[ii][j] != None:
            jump = 1
    jump = 0
    for ii in range(i+1, 4, 1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back:
                break
            elif 2 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].value and player_color == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color:
                was_ate = 1
                break
        elif a_map[ii][j] != None:
            jump = 1
    jump = 0
    for jj in range(j-1, -1, -1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back:
                break
            elif 2 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].value and player_color == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color:
                was_ate = 1
                break
        elif a_map[i][jj] != None:
            jump = 1
    jump = 0
    for jj in range(j+1, 8, 1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back:
                break
            elif 2 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].value and player_color == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color:
                was_ate = 1
                break
        elif a_map[i][jj] != None:
            jump = 1
    return was_ate
    
cdef list near(int i, int j):
    cdef list n_cor = []
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
    
cdef (int, int) mouse_position_to_block(int mx, int my, chess_back):
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

def near_max_value(open, org, a_map, my_chess):
    if None == open:
        return None
    (y, x) = open
    near_cor = near(y, x)
    cdef int max = 0
    for kk in near_cor:
        if kk == org:
            continue
        else:
            (ni, nj) = kk
            if a_map[ni][nj] != None:
                an = a_map[ni][nj]
                if my_chess[an[0]][an[1]].live == 1 and my_chess[an[0]][an[1]].back < 1:
                    if my_chess[an[0]][an[1]].color == com_color:
                        continue
                    if my_chess[an[0]][an[1]].value > max:
                        max = my_chess[an[0]][an[1]].value
    return max

def near_max_value_not_consider_com_color(open, org, a_map, my_chess):
    if None == open:
        return None
    (y, x) = open
    near_cor = near(y, x)
    cdef int max = 0
    for kk in near_cor:
        if kk == org:
            continue
        else:
            (ni, nj) = kk
            if a_map[ni][nj] != None:
                an = a_map[ni][nj]
                if my_chess[an[0]][an[1]].live == 1 and my_chess[an[0]][an[1]].back < 1:
                    #if my_chess[an[0]][an[1]].color == com_color:
                    #    continue
                    if my_chess[an[0]][an[1]].value > max:
                        max = my_chess[an[0]][an[1]].value
    return max    
    
def scan_player_bomb(a_map, my_chess):
    for cr in my_chess:
        for c in cr:
            if 1 == c.live and c.back < 1 and 2 == c.value and player_color == c.color:
                near_cor = near(c.row, c.col)
                i = random.randint(0, len(near_cor)-1)
                for ii in range(0, len(near_cor)):
                    n = (i+ii)%len(near_cor)
                    (ni, nj) = near_cor[n]
                    an = a_map[ni][nj]
                    if None == an:
                        continue
                    elif 0 == my_chess[an[0]][an[1]].live or my_chess[an[0]][an[1]].back < 1:
                        continue
                    if near_max_value(near_cor[n], (c.row, c.col), a_map, my_chess) <= 3 and 0 == eat_by_player_bomb(near_cor[n], a_map, my_chess, player_color):
                        return near_cor[n]
    return None

#def scan_com_second_big(a_map, my_chess):
#    for cr in my_chess:
#        for c in cr:
#            if 1 == c.live and c.back < 1 and 6 == c.value and com_color == c.color:
#                near_cor = near(c.row, c.col)
#                i = random.randint(0, len(near_cor)-1)
#                for ii in range(0, len(near_cor)):
#                    n = (i+ii)%len(near_cor)
#                    (ni, nj) = near_cor[n]
#                    an = a_map[ni][nj]
#                    if None == an:
#                        continue
#                    elif 0 == my_chess[an[0]][an[1]].live or my_chess[an[0]][an[1]].back < 1:
#                        continue
#                    if near_max_value(near_cor[n], (c.row, c.col), a_map, my_chess) <= 5:
#                        return near_cor[n]
#    return None

def bomb_will_eat(org, a_map, my_chess):
    if None == org:
        return None
    cdef int i = random.randint(0, 3)
    
    for i2 in range(0, 4):
        n = (i+i2)%4
        (ni, nj) = org
        if 0 == n:
            jump = 0
            for ii in range(ni-1, -1, -1):
                if 1 == jump and a_map[ii][nj] != None:
                    an = a_map[ii][nj]
                    if my_chess[an[0]][an[1]].back < 1 and my_chess[an[0]][an[1]].color == player_color:
                        if near_max_value((ii, nj), None, a_map, my_chess) < 2:
                            return org
                    break
                if a_map[ii][nj] != None:
                    bn = a_map[ii][nj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
        elif 1 == n:
            jump = 0
            for ii in range(ni+1, 4, 1):
                if 1 == jump and a_map[ii][nj] != None:
                    an = a_map[ii][nj]
                    if my_chess[an[0]][an[1]].back < 1 and my_chess[an[0]][an[1]].color == player_color:
                        if near_max_value((ii, nj), None, a_map, my_chess) < 2:
                            return org
                    break
                if a_map[ii][nj] != None:
                    bn = a_map[ii][nj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
        elif 2 == n:
            jump = 0
            for jj in range(nj-1, -1, -1):
                if 1 == jump and a_map[ni][jj] != None:
                    an = a_map[ni][jj]
                    if my_chess[an[0]][an[1]].back < 1 and my_chess[an[0]][an[1]].color == player_color:
                        if near_max_value((ni, jj), None, a_map, my_chess) < 2:
                            return org
                    break
                if a_map[ni][jj] != None:
                    bn = a_map[ni][jj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
        elif 3 == n:
            jump = 0
            for jj in range(nj+1, 8, 1):
                if 1 == jump and a_map[ni][jj] != None:
                    an = a_map[ni][jj]
                    if my_chess[an[0]][an[1]].back < 1 and my_chess[an[0]][an[1]].color == player_color:
                        if near_max_value((ni, jj), None, a_map, my_chess) < 2:
                            return org
                    break
                if a_map[ni][jj] != None:
                    bn = a_map[ni][jj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
    return None    
    
def bomb_may_eat(org, a_map, my_chess):
    if None == org:
        return None
    cdef int i = random.randint(0, 3)
    
    for i2 in range(0, 4):
        n = (i+i2)%4
        (ni, nj) = org
        if 0 == n:
            jump = 0
            for ii in range(ni-1, -1, -1):
                if 1 == jump and a_map[ii][nj] != None:
                    an = a_map[ii][nj]
                    if 1 == my_chess[an[0]][an[1]].back:
                        if near_max_value_not_consider_com_color((ii, nj), None, a_map, my_chess) < 2:
                            return (ii, nj)
                    break
                if a_map[ii][nj] != None:
                    bn = a_map[ii][nj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
        elif 1 == n:
            jump = 0
            for ii in range(ni+1, 4, 1):
                if 1 == jump and a_map[ii][nj] != None:
                    an = a_map[ii][nj]
                    if 1 == my_chess[an[0]][an[1]].back:
                        if near_max_value_not_consider_com_color((ii, nj), None, a_map, my_chess) < 2:
                            return (ii, nj)
                    break
                if a_map[ii][nj] != None:
                    bn = a_map[ii][nj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
        elif 2 == n:
            jump = 0
            for jj in range(nj-1, -1, -1):
                if 1 == jump and a_map[ni][jj] != None:
                    an = a_map[ni][jj]
                    if 1 == my_chess[an[0]][an[1]].back:
                        if near_max_value_not_consider_com_color((ni, jj), None, a_map, my_chess) < 2:
                            return (ni, jj)
                    break
                if a_map[ni][jj] != None:
                    bn = a_map[ni][jj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
        elif 3 == n:
            jump = 0
            for jj in range(nj+1, 8, 1):
                if 1 == jump and a_map[ni][jj] != None:
                    an = a_map[ni][jj]
                    if 1 == my_chess[an[0]][an[1]].back:
                        if near_max_value_not_consider_com_color((ni, jj), None, a_map, my_chess) < 2:
                            return (ni, jj)
                    break
                if a_map[ni][jj] != None:
                    bn = a_map[ni][jj]
                    if 1 == my_chess[bn[0]][bn[1]].back:
                        jump = 1
                    else:
                        break
    return None
    
def scan_com_bomb(a_map, my_chess):
    cor = None
    for cr in my_chess:
        for c in cr:
            if 1 == c.live and c.back < 1 and 2 == c.value and com_color == c.color:
                cor = bomb_may_eat((c.row, c.col), a_map, my_chess)
    return cor

#def scan_open_bomb(a_map, my_chess):
#    cor = None
#    for s_num in range(0, 32):
#        s = random.randint(0, 31)
#        j = s/4
#        i = s%4
#        a = a_map[i][j]
#        if None == a:
#            continue
#        m = my_chess[a[0]][a[1]]
#        if m.back < 1 or 0 == m.live:
#            continue
#        cor = bomb_will_eat((i,j), a_map, my_chess)
#        if None == cor:
#            continue
#        ncor = near(a[0], a[1])
#        for nc in ncor:
#            b = a_map[nc[0]][nc[1]]
#            if None == b:
#                continue
#            mn = my_chess[b[0]][b[1]]
#            if 0 == mn.live or None == mn:
#                continue
#            if mn.back < 1:
#                if mn.color == player_color and mn.value > 2:
#                    cor = None
#                    break
#        if cor != None:
#            return cor
#    return cor
    
def select_back_chess(a_map, my_chess, org = None):
    global player_color
    global com_color
    
    (i, j) = (None, None)
    
    cor = scan_player_bomb(a_map, my_chess)
    if cor != None:
        return cor
    
    cor = scan_com_bomb(a_map, my_chess)
    if cor != None:
        return cor
    
    randomi = random.randint(0, 1)
    if 0 == randomi:
        y0 = 0
        y1 = 4
        y2 = 1
    else:
        y0 = 3
        y1 = -1
        y2 = -1
        
    randomi = random.randint(0, 1)
    if 0 == randomi:
        x0 = 0
        x1 = 8
        x2 = 1
    else:
        x0 = 7
        x1 = -1
        x2 = -1
    
    (i, j) = calc_good_backchess(y0, y1, y2, x0, x1, x2, a_map, my_chess, max_eat_number = 0)
                    
    if (i, j) != (None, None):
        return (i, j)
    elif org != None:
        #  (-1, -1) to move a piece
        return (-1, -1)
    else:
        if 1 == check_back_exist(a_map, my_chess):
            return calc_good_backchess(y0, y1, y2, x0, x1, x2, a_map, my_chess)
        else:
            return None

cdef int check_back_exist(a_map, my_chess):
    cdef int back_exist = 0
    for i in range(0, 4):
        for j in range(0, 8):
            if a_map[i][j] != None:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].back:
                    #back_exist = 1
                    return 1
    return back_exist

# return (i, j) won't be (None, None) if max_eat_number = -33
def calc_good_backchess(int y0, int y1, int y2, int x0, int x1, int x2, a_map, my_chess, int max_eat_number = -33):
    global back_num
    
    (i, j) = (None, None)
    
    for y in range(y0, y1, y2):
        for x in range(x0, x1, x2):
            near_min = 8
            near_max = 0
            near_our_min = 8
            near_our_max = 0
            
            if None == a_map[y][x]:
                continue
            
            n = a_map[y][x]
            if my_chess[n[0]][n[1]].back < 1:
                continue
                
            if 0 == eat_by_player_bomb((y, x), a_map, my_chess, player_color):
                near_cor = near(y, x)
                for kk in near_cor:
                    (ni, nj) = kk
                    if a_map[ni][nj] != None:
                        an = a_map[ni][nj]
                        if my_chess[an[0]][an[1]].back < 1:
                            if my_chess[an[0]][an[1]].value < near_min and player_color ==  my_chess[an[0]][an[1]].color:
                                near_min = my_chess[an[0]][an[1]].value
                            if my_chess[an[0]][an[1]].value > near_max and player_color == my_chess[an[0]][an[1]].color:
                                near_max = my_chess[an[0]][an[1]].value
                            if my_chess[an[0]][an[1]].value < near_our_min and com_color ==  my_chess[an[0]][an[1]].color:
                                near_our_min = my_chess[an[0]][an[1]].value
                            if my_chess[an[0]][an[1]].value > near_our_max and com_color ==  my_chess[an[0]][an[1]].color:
                                near_our_max = my_chess[an[0]][an[1]].value
                ne = check_eat_number(a_map, my_chess, near_min, near_max, near_our_min, near_our_max, y, x)
                if ne > max_eat_number:
                    max_eat_number = ne
                    (i, j) = (y, x)
            elif -33 == max_eat_number:
                max_eat_number = -1 * back_num
                (i, j) = (y, x)
    return (i, j)
    
#def random_select_back_chess(a_map, my_chess):    
#    i = random.randint(0, 31)
#    ii = 0
#    
#    while i != -1:
#        y = ii/8
#        x = ii%8
#        if a_map[y][x] == None:
#            ii += 1
#            if ii > 31:
#                ii = 0
#        elif 1 == my_chess[a_map[y][x][0]][a_map[y][x][1]].back:
#            i -= 1
#            if i < 0:
#                break
#            ii += 1
#            if ii > 31:
#                ii = 0
#        else:
#            ii += 1
#            if ii > 31:
#                ii = 0
#    
#    return (y, x)
                
def chess_ai():
    global turn_id
    global first
    global main_chess
    global main_map
    global player_color
    global com_color
    global player_first
    global player_win
    global back_num
    #global com_will_eat_chess
    #global will_eat_escape_chess
    global step
    
    pygame.display.update()
    
    if 0 == player_first and  1 == first:
        i = random.randint(0, 3) 
        j = random.randint(0, 7)
        cindex = color_value_to_index(server_main_chess[i][j].color, server_main_chess[i][j].value, back_value_num)
        main_chess[i][j] = chess(cindex, (i, j))
        turn_id = main_chess[i][j].color
        main_chess[i][j].back = -1
        back_num -= 1
        com_color = turn_id
        player_color = 1 - com_color
        first = 0
        back_value_num[com_color][main_chess[i][j].value] -= 1
        #print(back_value_num)
    elif turn_id == com_color and 0 == first:
        #com_will_eat_chess = []
        #will_eat_escape_chess = [] 
        main_chess = clean_back_n1_to_0(main_chess)
        
        move_pre1 = move_step[(sindex-1)%4] # player
        move_pre2 = move_step[(sindex-2)%4] # com
        move_pre3 = move_step[(sindex-3)%4] # player
        move_pre4 = move_step[sindex]       # com
                
        if move_pre1 != None and move_pre2 != None and move_pre3 != None and move_pre4 != None:
            if move_pre1[0] == player_color and move_pre2[0] == com_color and move_pre3[0] == player_color and move_pre4[0] == com_color and in_com_possible_move(move_pre1[1], move_pre2[3])and in_com_possible_move(move_pre3[1], move_pre4[3]) and move_pre2[2] == move_pre4[1] and move_pre1[2] == move_pre3[1]:
                n1 = move_pre1[1]
                n2 = move_pre2[1]
                p  = move_pre1[2]
                c  = move_pre2[2]
                break_long_capture_dest.append([n1, n2, p, c])
                break_long_capture_org.append([p, c])
                com_ban_step.append(move_pre4[1])
        
        org, dest, score = com_think(main_map, main_chess)
        #print 'org', org, 'dest', dest, 'score', score, 'op score', open_score
                            
        if 0 == back_num and 1 == cant_move(main_map, main_chess, com_color):
            player_win = 1
        if back_num > 0:
            if open_score != None:
                if None == org:
                    dest = select_back_chess(main_map, main_chess)
                    sound_click = pygame.mixer.Sound(s_click)
                    sound_click.play()
                    sc = server_main_chess[main_map[dest[0]][dest[1]][0]][main_map[dest[0]][dest[1]][1]]
                    cindex = color_value_to_index(sc.color, sc.value, back_value_num)
                    main_chess[main_map[dest[0]][dest[1]][0]][main_map[dest[0]][dest[1]][1]] = chess(cindex, (main_map[dest[0]][dest[1]][0], main_map[dest[0]][dest[1]][1]))
                    m = main_chess[main_map[dest[0]][dest[1]][0]][main_map[dest[0]][dest[1]][1]]
                    m.back = -1
                    back_num -= 1
                    back_value_num[m.color][m.value] -= 1
                    #print(back_value_num)
                elif score > open_score:
                    if score > 18:
                        org = None
                    temp = select_back_chess(main_map, main_chess, org)
                    if (-1, -1) == temp:
                        main_map, main_chess, a_map = move_s(org, dest, main_map, main_chess)
                        save_step_and_break_long_capture(org, dest, a_map, main_chess)
                    else:
                        sound_click = pygame.mixer.Sound(s_click)
                        sound_click.play()
                        sc = server_main_chess[main_map[temp[0]][temp[1]][0]][main_map[temp[0]][temp[1]][1]]
                        cindex = color_value_to_index(sc.color, sc.value, back_value_num)
                        main_chess[main_map[temp[0]][temp[1]][0]][main_map[temp[0]][temp[1]][1]] = chess(cindex, (main_map[temp[0]][temp[1]][0], main_map[temp[0]][temp[1]][1]))
                        m = main_chess[main_map[temp[0]][temp[1]][0]][main_map[temp[0]][temp[1]][1]]
                        m.back = -1
                        back_num -= 1
                        back_value_num[m.color][m.value] -= 1
                        #print(back_value_num)
                elif score == open_score:
                    if score >= 0:
                        if score > 18:
                            org = None
                        temp = select_back_chess(main_map, main_chess, org)
                        if (-1, -1) == temp:
                            main_map, main_chess, a_map = move_s(org, dest, main_map, main_chess)
                            save_step_and_break_long_capture(org, dest, a_map, main_chess)
                        else:
                            sound_click = pygame.mixer.Sound(s_click)
                            sound_click.play()
                            sc = server_main_chess[main_map[temp[0]][temp[1]][0]][main_map[temp[0]][temp[1]][1]]
                            cindex = color_value_to_index(sc.color, sc.value, back_value_num)
                            main_chess[main_map[temp[0]][temp[1]][0]][main_map[temp[0]][temp[1]][1]] = chess(cindex, (main_map[temp[0]][temp[1]][0],main_map[temp[0]][temp[1]][1]))
                            m = main_chess[main_map[temp[0]][temp[1]][0]][main_map[temp[0]][temp[1]][1]]
                            m.back = -1
                            back_num -= 1
                            back_value_num[m.color][m.value] -= 1
                            #print(back_value_num)
                    else:
                        main_map, main_chess, a_map = move_s(org, dest, main_map, main_chess)
                        save_step_and_break_long_capture(org, dest, a_map, main_chess)
                else:
                    main_map, main_chess, a_map = move_s(org, dest, main_map, main_chess)
                    save_step_and_break_long_capture(org, dest, a_map, main_chess)
            else:
                main_map, main_chess, a_map = move_s(org, dest, main_map, main_chess)
                save_step_and_break_long_capture(org, dest, a_map, main_chess)
        elif 0 == player_win:
            main_map, main_chess, a_map = move_s(org, dest, main_map, main_chess)
            save_step_and_break_long_capture(org, dest, a_map, main_chess)
   
    if turn_id == com_color:
        step += 1
        print("step = %d" % step)
        turn_id = 2

cdef int f_short_dist(int i, int j, dist, a_map):
    d = 0
    ncor = near(i, j)
    for nc in ncor:
        if mark[nc[0]][nc[1]] != 0 and mark[nc[0]][nc[1]] < dist:
            if 0 == d:
                d = mark[nc[0]][nc[1]]+1
            elif mark[nc[0]][nc[1]]+1 < d:
                d = mark[nc[0]][nc[1]]+1
    
    if 0 == d and None == a_map[i][j]:
        d = dist
    
    return d

cdef double f_calc_move_score(double max_value, double max_dist, double my_value):
    
    mvalue = my_value/11
    
    if 9 == max_value:
        
        if max_dist != 0:
            if 3.5 > 0.2 * max_dist:
                # return 3.5 - 0.2 * max_dist + 0.7
                return 4.2 - 0.2 * max_dist
            else:
                return 0.7 - max_dist/1000
        else:
            # impossible
            return 0
    else:
        if max_value != 0:
            if max_value/2 > 0.2 * max_dist:
                return max_value/2 - 0.2 * max_dist + mvalue
            else:
                return mvalue - max_dist/1000
        else:
            return -0.1

cdef void first_move_max_value(int orgx, int orgy, int destx, int desty, my_chess, a_map, int org_value, int owner_color, int i, int j, int dist=1):
    global max_value
    global mark
    #global max_cor
    global cannon_mark
    global max_dist
    
    if i == -1 or j == -1 or i == 4 or j == 8:
        return
    elif i == orgy and j == orgx:
        return
    #elif 1 == mark[i][j]:
    elif mark[i][j] > 0 or cannon_mark[i][j] > 0:
        return
        
    n_c = near(i, j)
    
    for nc in n_c:
        (ni, nj) = nc
        if a_map[ni][nj] != None:
            an = a_map[ni][nj]
            if my_chess[an[0]][an[1]].live == 1 and my_chess[an[0]][an[1]].back < 1 and my_chess[an[0]][an[1]].color != owner_color:
                if 1 == can_be_ate(my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value, my_chess[an[0]][an[1]].value):
                    return
    
    if a_map[i][j] != None:
        if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].back:
            return
        #elif owner_color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
        #    return
    
    opp_color = 1 - owner_color
    current_dist = 32
    
    if  a_map[i][j] != None:
        current_dist = f_short_dist(i, j, dist, a_map)
    else:
        mark[i][j] = f_short_dist(i, j, dist, a_map)
    
    if a_map[i][j] != None:
        if opp_color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
            if 7 == org_value:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    #max_value = 8
                    #max_cor = (i, j)
                    return
                elif 2 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and max_value <= 5.5:
                    if max_value < 5.5:
                        max_value = 5.5
                        max_dist = current_dist
                    elif current_dist < max_dist:
                        max_dist = current_dist
                    return
                elif max_value <= my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    if max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                        max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                        max_dist = current_dist
                    #max_cor = (i, j)
                    elif current_dist < max_dist:
                        max_dist = current_dist
                    return
            elif 1 == org_value:
                if 7 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    if max_value != 9:
                        max_value = 9
                        max_dist = current_dist
                    #max_cor = (i, j)
                    elif current_dist < max_dist:
                        max_dist = current_dist
                    return
                elif my_chess[a_map[i][j][0]][a_map[i][j][1]].value == 1:
                    if max_value != 1:
                        max_value = 1
                        max_dist = current_dist
                    #max_cor = (i, j)
                    elif current_dist < max_dist:
                        max_dist = current_dist
                    return
            elif 2 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and org_value > 2 and max_value <= 5.5:
                if max_value < 5.5:
                    max_value = 5.5
                    max_dist = current_dist
                elif current_dist < max_dist:
                    max_dist = current_dist
                return
            elif max_value <= my_chess[a_map[i][j][0]][a_map[i][j][1]].value and my_chess[a_map[i][j][0]][a_map[i][j][1]].value <= org_value:
                if max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                    max_dist = current_dist
                #max_cor = (i, j)
                elif current_dist < max_dist:
                    max_dist = current_dist
                return
    elif orgy == desty and orgx+1 == destx:
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j, dist+1)   
    elif orgy == desty and orgx-1 == destx:
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j, dist+1)
    elif orgy+1 == desty and orgx == destx:
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1, dist+1)
    elif orgy-1 == desty and orgx == destx:
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1, dist+1)
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1, dist+1)

# return 0: NOT caca, 1: caca, 2:equal, 3: same row or column with equal value       
cdef int caca(org, dest, my_chess, a_map, int owner_color):        
    if org == None:
        return 0
    elif owner_color == player_color:
        return 0
    
    (orgy, orgx) = org
    (desty, destx) = dest
    
    m = a_map[orgy][orgx]
    if m == None:
        return 0
    elif 2 == my_chess[m[0]][m[1]].value:
        return 0
    
    if desty-1 >= 0 and destx-1 >=0:
        n = a_map[desty-1][destx-1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass                
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 1 == eat_value:
                    return 1
                elif 2 == eat_value:
                    return 2
    if desty-1 >= 0 and destx+1 <=7:
        n = a_map[desty-1][destx+1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 1 == eat_value:
                    return 1
                elif 2 == eat_value:
                    return 2
    if desty+1 <= 3 and destx-1 >= 0:
        n = a_map[desty+1][destx-1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 1 == eat_value:
                    return 1
                elif 2 == eat_value:
                    return 2
    if desty+1 <= 3 and destx+1 <= 7:
        n = a_map[desty+1][destx+1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 1 == eat_value:
                    return 1
                elif 2 == eat_value:
                    return 2
                    
    if desty-2 >= 0:
        n = a_map[desty-2][destx]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass                
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 2 == eat_value:
                    return 3
    if desty+2 <= 3:
        n = a_map[desty+2][destx]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 2 == eat_value:
                    return 3
    if destx-2 >= 0:
        n = a_map[desty][destx-2]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 2 == eat_value:
                    return 3
    if destx+2 <= 7:
        n = a_map[desty][destx+2]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                eat_value = can_be_ate_equal(my_chess[n[0]][n[1]].value, my_chess[m[0]][m[1]].value)
                if 2 == eat_value:
                    return 3
                    
    return 0
        
        
cdef int near2_have_same_value(org, my_chess, a_map, int owner_color):
    if org == None:
        return 0
    elif owner_color == player_color:
        return 0
    
    (orgy, orgx) = org
    
    m = a_map[orgy][orgx]
    if m == None:
        return 0
    elif 2 == my_chess[m[0]][m[1]].value:
        return 0
    
    if orgy-2 >= 0:
        n = a_map[orgy-2][orgx]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgy+2 <= 3:
        n = a_map[orgy+2][orgx]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgx-2 >= 0:
        n = a_map[orgy][orgx-2]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgx+2 <= 7:
        n = a_map[orgy][orgx+2]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgy-1 >= 0 and orgx-1 >=0:
        n = a_map[orgy-1][orgx-1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgy-1 >= 0 and orgx+1 <=7:
        n = a_map[orgy-1][orgx+1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgy+1 <= 3 and orgx-1 >= 0:
        n = a_map[orgy+1][orgx-1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    if orgy+1 <= 3 and orgx+1 <= 7:
        n = a_map[orgy+1][orgx+1]
        if n == None:
            pass
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                pass
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                pass
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            #elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
            #    return 1
    return 0

cdef void scan_king(my_chess):
    global king_live
    
    for chr in my_chess:
        for ch in chr:
            if 7 == ch.value:
                king_live[ch.color] = ch.live

cdef bint in_com_possible_move(org, possible_mv):
    #for pm in possible_mv:
    #    if org == pm:
    #        return True
    if org in possible_mv:
        return True
            
    return False
                
cdef void save_step_and_break_long_capture(org, dest, a_map, my_chess):
    global move_step
    global sindex
    global break_long_capture_dest
    global break_long_capture_org
    global com_ban_step
    
    if org != dest:
        possible_mv = collect_possible_move(dest[0], dest[1], a_map, my_chess)
        move_step[sindex] = [com_color, org, dest, possible_mv]
        sindex = (sindex+1)%4
        
        # Simon added 20191016
        (row, col) = dest
        for i in range(-2, 3):
            for j in range(-2, 3):
                if abs(i)+abs(j) <= 2 and 0 <= row + i <= 3 and 0 <= col + j <= 7:
                    br = 0
                    dt = (row+i, col+j)
                    while(br < len(break_long_capture_dest)):
                        b = 0
                        if dt in break_long_capture_dest[br]:
                            del break_long_capture_dest[br]
                            del break_long_capture_org[br]
                            del com_ban_step[br]
                            b = 1
                        if 0 == b:
                            br += 1
        # End Simon added 20191016
                                    
        #br = 0
        #while(br < len(break_long_capture_dest)):
        #    b = 0
        #    for d in break_long_capture_dest[br]:
        #        if dest == d:
        #            del break_long_capture_dest[br]
        #            del break_long_capture_org[br]
        #            del com_ban_step[br]
        #            b = 1
        #            break
        #    if 0 == b:
        #        br += 1
                
        br = 0
        while(br < len(break_long_capture_org)):
            b = 0
            for o in break_long_capture_org[br]:
                if org == o:
                    del break_long_capture_dest[br]
                    del break_long_capture_org[br]
                    del com_ban_step[br]
                    b = 1
                    break
            if 0 == b:
                br += 1

def calc_cannon_mark(my_chess, a_map, int owner_color):
    cannon_mark = [[0]*8, [0]*8, [0]*8, [0]*8]
    
    find_player_cannon_num = 0
    jump = 0
    
    if owner_color == player_color:
        return cannon_mark
    
    for r in range(0, 4):
        for c in range(0, 8):
            if 2 == find_player_cannon_num:
                break
            if 1 == my_chess[r][c].live and 0 == my_chess[r][c].back and player_color == my_chess[r][c].color and 2 == my_chess[r][c].value:
                find_player_cannon_num += 1
                (i, j) = (my_chess[r][c].row, my_chess[r][c].col)
                for ii in range(i-1, -1, -1):
                    if 1 == jump and a_map[ii][j] != None:
                        break
                    elif 1 == jump and None == a_map[ii][j]:
                        cannon_mark[ii][j] = 1
                    elif a_map[ii][j] != None:
                        jump = 1
                jump = 0
                for ii in range(i+1, 4, 1):
                    if 1 == jump and a_map[ii][j] != None:
                            break
                    elif 1 == jump and None == a_map[ii][j]:
                        cannon_mark[ii][j] = 1
                    elif a_map[ii][j] != None:
                        jump = 1
                jump = 0
                for jj in range(j-1, -1, -1):
                    if 1 == jump and a_map[i][jj] != None:
                        break
                    elif 1 == jump and None == a_map[i][jj]:
                        cannon_mark[i][jj] = 1
                    elif a_map[i][jj] != None:
                        jump = 1
                jump = 0
                for jj in range(j+1, 8, 1):
                    if 1 == jump and a_map[i][jj] != None:
                        break
                    elif 1 == jump and None == a_map[i][jj]:
                        cannon_mark[i][jj] = 1
                    elif a_map[i][jj] != None:
                        jump = 1
                  
    return cannon_mark

cdef double first_move_score(org, dest, my_chess, a_map, int owner_color, int player_color, int com_color, list com_ban_step, king_live):
    global max_value
    global mark
    global max_dist
    global cannon_mark
    
    if org == dest or None == org or None == dest:
        return 0
    
    (orgy, orgx) = org
    (desty, destx) = dest
       
    if a_map[desty][destx] == None:
        #for ban in com_ban_step:
        #    if org == ban:
        #        return -10.2
        if org in com_ban_step:
            return -10.2
        
        ndead = escape_way_to_run(org, dest, my_chess, a_map, owner_color)
        
        if owner_color == player_color:
            if 1 == will_eat2_more(org, dest, my_chess, a_map, owner_color):
                return 8
            return 0
        elif 0 == dest_will_dead_owner_wont_eat(org, dest, main_chess, main_map, player_color) and 1 == stand_will_dead_pity((orgy, orgx), main_chess, main_map, com_color):
            return 9 + ndead
        
        if  2 == my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value:
            af_map = copy.deepcopy(a_map)
            af_ch = copy.deepcopy(my_chess)
            if org != None and dest != None:
                af_map, af_ch = move(org, dest, af_map, af_ch)
                all_chess_move(af_map, af_ch)
                cannon = af_ch[af_map[dest[0]][dest[1]][0]][af_map[dest[0]][dest[1]][1]]
                for pm in cannon.possible_move:
                    (pmy, pmx) = pm
                    am = af_map[pmy][pmx]
                    if None == am:
                        continue
                    c = af_ch[am[0]][am[1]]
                    if c.value > 5:
                        return 7.3           
            return 0
        
        max_value = 0
        max_dist = 32
        #max_cor = None
        #mark = [[0]*8, [0]*8, [0]*8, [0]*8]
        mark[0][:] = [0, 0, 0, 0, 0, 0, 0, 0]
        mark[1][:] = [0, 0, 0, 0, 0, 0, 0, 0]
        mark[2][:] = [0, 0, 0, 0, 0, 0, 0, 0]
        mark[3][:] = [0, 0, 0, 0, 0, 0, 0, 0]

        org_value = my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value
                
        mvalue = 0
        
        if a_map[orgy][orgx] != None:
                mp = a_map[orgy][orgx]
                mvalue = my_chess[mp[0]][mp[1]].value
                
        cannon_mark = calc_cannon_mark(my_chess, a_map, owner_color)
        
        first_move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color, desty, destx)
        
        cvalue = caca(org, dest, my_chess, a_map, owner_color)
        
        #print('cvalue', cvalue, org, dest) ##temp
        
        if 1 == cvalue:
            return f_calc_move_score(max_value, max_dist, mvalue) + 0.1
        elif 2 == cvalue:
            return f_calc_move_score(max_value, max_dist, mvalue) + 0.3
        elif 3 == cvalue:
            return f_calc_move_score(max_value, max_dist, mvalue) + 0.28
        
        if 1 == near2_have_same_value(org, my_chess, a_map, owner_color):
            if 0 == will_dead_pity_even_equal(org, dest, my_chess, a_map, owner_color):
                return -0.1
        
        ncor = near(orgy, orgx)
        for nc in ncor:
            if a_map[nc[0]][nc[1]] != None:
                a = a_map[nc[0]][nc[1]]
                small_value = my_chess[a[0]][a[1]].value
                if 1 == my_chess[a[0]][a[1]].back:
                    continue
                if player_color == my_chess[a[0]][a[1]].color and 1 == can_be_ate(small_value, org_value):
                    return -0.1
        
        #############temp
        #if (0, 5) == org and (1, 5) == dest:
        #    print("ndead =", ndead, "r =", f_calc_move_score(max_value, max_dist, mvalue) + ndead)
        #############end temp
        
        return f_calc_move_score(max_value, max_dist, mvalue) + ndead
    
    #elif 1 == my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].live:
    else:
        ndead = escape_way_to_run(org, dest, my_chess, a_map, owner_color)
        
        org_score = eating_value_to_score(my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].value, king_live, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color)
        
        if 0 == ndead:    
            return org_score + 10
        else:   
            return org_score

                
cdef double move_score(org, dest, my_chess, a_map, int owner_color, int player_color, int com_color, list com_ban_step, king_live, int step = 1):
    
    if org == dest or None == org or None == dest:
        return 0
    
    (orgy, orgx) = org
    (desty, destx) = dest
    
    if a_map[desty][destx] == None:
        #for ban in com_ban_step:
        #    if org == ban:
        #        return -10.2
        if org in com_ban_step:
            return -10.2
        
        ndead = escape_way_to_run(org, dest, my_chess, a_map, owner_color)
        if step > 2 and 0 == ndead:
            # marked 20201217
            #if 1 == opp_cannon_can_eat(org, dest, my_chess, a_map):
            #    return 7.5
            if a_map[orgy][orgx] != None:
                m = a_map[orgy][orgx]
                if 3 == my_chess[m[0]][m[1]].value:
                    return 7
                else:
                    #if v != None:
                    return 10
        elif owner_color == player_color:
            if 1 == will_eat2_more(org, dest, my_chess, a_map, owner_color):
                return 8
            return 0
        elif 0 == dest_will_dead_owner_wont_eat(org, dest, main_chess, main_map, player_color) and 1 == stand_will_dead_pity((orgy, orgx), main_chess, main_map, com_color):
            return 9 + ndead        
        
        if  2 == my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value:
            af_map = copy.deepcopy(a_map)
            af_ch = copy.deepcopy(my_chess)
            if org != None and dest != None:
                af_map, af_ch = move(org, dest, af_map, af_ch)
                all_chess_move(af_map, af_ch)
                cannon = af_ch[af_map[dest[0]][dest[1]][0]][af_map[dest[0]][dest[1]][1]]
                for pm in cannon.possible_move:
                    (pmy, pmx) = pm
                    am = af_map[pmy][pmx]
                    if None == am:
                        continue
                    c = af_ch[am[0]][am[1]]
                    if c.value > 5:
                        return 7.3           
            return 0
        
        return 0
    
    #elif 1 == my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].live:
    else:
        ndead = escape_way_to_run(org, dest, my_chess, a_map, owner_color)
        
        org_score = eating_value_to_score(my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].value, king_live, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color)
        
        if 0 == ndead:    
            return org_score + 10
        else:   
            return org_score

def move_s(org, dest, a_map, a_ch):
    #global cor
    global com_mv_map
    
    (orgi, orgj) = org
    (desti, destj) = dest
    
    af_map = copy.deepcopy(a_map)
    
    #if org == dest:
    #    print("crash")
    
    if None == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        com_mv_map = list(a_map[orgi][orgj])        
        af_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        af_map[orgi][orgj] = None
        a_map[orgi][orgj] = None
        sound_move = pygame.mixer.Sound(s_move2)
        sound_move.play()
    else:
        #dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        #dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        com_mv_map = list(a_map[orgi][orgj])        
        af_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        af_map[orgi][orgj] = None
        a_map[orgi][orgj] = None
        sound_capture = pygame.mixer.Sound(s_capture)
        sound_capture.play()
    
    return a_map, a_ch, af_map    
        
def move(org, dest, a_map, a_ch):
    #global cor
    
    if org == dest or None == org or None == dest:
        return a_map, a_ch
    
    (orgi, orgj) = org
    (desti, destj) = dest
    
    if None == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        a_map[orgi][orgj] = None
    else:
        dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        a_map[orgi][orgj] = None
    
    return a_map, a_ch    
    
cdef int cant_move(a_map, a_ch, int owner_color):
    all_chess_move(a_map, a_ch)
    for chr in a_ch:
        for ch in chr:
            if ch.color == owner_color and 1 == ch.live:
                #for pm in ch.possible_move:
                if ch.possible_move:
                    return 0
    return 1
    
def com_think(a_map, a_ch):
    global open_score
    global final_score
    global gb_m2
    global com_ban_step
    global king_live
    global max_value
    global max_dist
    global mark

    cdef list m = []
    
    cdef double min_score = 9000
    cdef double sc = 0
    
    all_chess_move(a_map, a_ch)
    
    scan_king(a_ch)
    
    if back_num > 0:
        open_score = 0.01
        m.append((None, None, 0.01, 0))
        min_score = 0.01
        org = None
        dest = None
    else:
        open_score = None
    
    for chr in a_ch:
        for ch in chr:
            if 1 == ch.back or 0 == ch.live:
                continue
            if ch.color == com_color:
                for pm in ch.possible_move:
                    pity = 0
                    if 0 == will_dead_pity((ch.row, ch.col), pm, a_ch, a_map, com_color):                        
                        score = sc - first_move_score((ch.row, ch.col), pm, a_ch, a_map, com_color, player_color, com_color, com_ban_step, king_live)
                                               
                    else:
                        self_score = eating_value_to_score(ch.value, king_live, com_color) * 0.2
                        
                        score = sc + 40 + self_score - first_move_score((ch.row, ch.col), pm, a_ch, a_map, com_color, player_color, com_color, com_ban_step, king_live)
                        pity = 1
                    
                    m.append(((ch.row, ch.col), pm, score, pity))
                    
                    if score < min_score:
                        min_score = score
                        org = (ch.row, ch.col)
                        dest = pm   
    
    alpha = AI_min_score #mini
    final_score = AI_min_score #mini and final score
    min_index = None # final score index
    
    beta = -1 *  AI_min_score#max
    
    #print('m', m) ##temp
    
    if len(m) > 1:
        mf = []
        threads = []
        i = 0
        mnum = len(m)
        gb_m2 = [None] * mnum
        q = Queue()
  
        for mm in m:
            #print('mm', mm)
            
            threads.append(Process(target = one_turn, args = (q, a_map, a_ch, mm, player_color, mm[0], mm[1], mm[2], mm[3], 0.90, i, alpha, beta, player_color, com_color, back_num, com_ban_step, king_live, gb_m2)))
            threads[i].start()
            #threads[i].join()
            i += 1
            
        #while threading.activeCount() > 1:
        nump = 0
        while nump < len(m):
            for i in range(mnum):
                gb_m2[i] = q.get()
                if gb_m2[i] is not None:
                    nump += 1
                    
                    # if nexti == nextj:
                    if gb_m2[i][0][0] == gb_m2[i][0][1]:
                        open_score = gb_m2[i][0][4]
                    
                    if final_score > gb_m2[i][0][4]:
                        final_score = gb_m2[i][0][4]
                        min_index = len(mf)
                        
                    mf.append([gb_m2[i][0][0], gb_m2[i][0][1], gb_m2[i][0][4]])
                    gb_m2[i] = None
                    
            for event in pygame.event.get():
                    if event.type == QUIT:
                        exit()
        
        if mf:
            print('mf=', mf)
            #min_index = mf.index(min(mf, key=lambda s:s[2]))
            return mf[min_index][0], mf[min_index][1], mf[min_index][2]
        else:
            return org, dest, min_score
    elif 1 == len(m):
        return m[0][0], m[0][1], m[0][2]
    else:
        return None, None, 0     

# extend one_turn to 2-level-deep
# original one_turn for player(next to com player)
# extend to player-com-player
def one_turn(q, a_map, a_ch, mm, int owner_color, nexti, nextj, double sc, int pt, double div, int ind, double alpha, double beta, int player_color, int com_color, int back_num, list com_ban_step, king_live, list gb_m2):
    
    cdef double max_p_score = -9000
    div2 = 0.901
    
    cdef list m2 = []
    cdef list m3 = []
    cdef list m4 = []
    
    for ban in com_ban_step:
        if nexti == ban:
            pt = 1
    
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    if nexti != None and nextj != None:
        af_map, af_ch = move(nexti, nextj, af_map, af_ch)
        all_chess_move(af_map, af_ch)
    
    if owner_color == player_color and 1 == cant_move(af_map, af_ch, player_color) and 0 == back_num:
        m2.append([mm[0], mm[1], None, None, max_p_score])
        #m2.append([mm[0], mm[1], None, None, sc])
        
        gb_m2[ind] = m2
        q.put(gb_m2[ind])
        
        return
        #return m2, af_map, af_ch
    
    if back_num > 0:
        all_pm = [[None, None]]
    else:
        all_pm = []
    for chr in af_ch:
        for ch in chr:
            if ch.color == owner_color and 1 == ch.live and ch.back < 1:
                for apm in ch.possible_move:
                    all_pm.append([(ch.row, ch.col), apm])
                score = sc
            
    for ch_position, pm in all_pm:
        # To avoid "Not Responding" 20200923
        for event in pygame.event.get():
            if event.type == QUIT:
                exit()
        # End 20200923
    
        mscore =  move_score(ch_position, pm, af_ch, af_map, player_color, player_color, com_color, com_ban_step, king_live, 2)
        
        score = sc + div2 * mscore

        #############################
        af_map_2 = copy.deepcopy(af_map)
        af_ch_2 = copy.deepcopy(af_ch)
        if ch_position != None and pm != None:
            af_map_2, af_ch_2 = move(ch_position, pm, af_map_2, af_ch_2)
            all_chess_move(af_map_2, af_ch_2)
        
        #if back_num > 0:
        #    m3.append([(ch.row, ch.col), pm, None, None, 10])
        if back_num > 0:
            all_pm_2 = [[None, None]]
        else:
            all_pm_2 = []
            
        for chr_com in af_ch_2:
            for ch_com in chr_com:                        
                if ch_com.color == 1 - owner_color and 1 == ch_com.live and ch_com.back < 1:
                    for apm_com in ch_com.possible_move:
                        all_pm_2.append([(ch_com.row, ch_com.col), apm_com])
        
        for ch_position2, pm_com in all_pm_2:
            if 1 == pt and ch_position2 == nextj:
                score2 = score
            else:
                score2 = score - div * move_score(ch_position2, pm_com, af_ch_2, af_map_2, com_color, player_color, com_color, com_ban_step, king_live, 3)
            
            #if score2 > final_score:
            #    continue
                            
            ###############################
            af_map_3 = copy.deepcopy(af_map_2)
            af_ch_3 = copy.deepcopy(af_ch_2)
            if ch_position2 != None and pm_com != None:                                        
                af_map_3, af_ch_3 = move(ch_position2, pm_com, af_map_3, af_ch_3)
                all_chess_move(af_map_3, af_ch_3)
            
            #if back_num > 0:
            #    m4.append([(ch_com.row, ch_com.col), pm_com, None, None, 10])
            if back_num > 0:
                all_pm_3 = [[None, None]]
            else:
                all_pm_3 = []
            
            for chr_p in af_ch_3:
                for ch_p in chr_p:
                    if ch_p.color == owner_color and 1 == ch_p.live and ch_p.back < 1:
                        for apm_p in ch_p.possible_move:
                            all_pm_3.append([(ch_p.row, ch_p.col), apm_p])
                                
            for ch_position3, pm_p in all_pm_3:
                pity = will_dead_pity(ch_position3, pm_p, af_ch_3, af_map_3, owner_color)
                
                if 0 == pity:
                    
                    if 0 == will_dead_pity_even_equal(ch_position3, pm_p, af_ch_3, af_map_3, owner_color):#equal
                        p_a = af_map_3[ch_position3[0]][ch_position3[1]]
                        #if None == p_a:
                        #    print(crash)
                        c_a = af_ch_3[p_a[0]][p_a[1]]
                            
                        score3 = score2 + div * move_score(ch_position3, pm_p, af_ch_3, af_map_3, player_color, player_color, com_color, com_ban_step, king_live, 4)
                        
                        #bomb_score = eating_value_to_score(c_a.value, king_live, c_a.color)
                        bomb_score = 290 # fast than function return
                        
                        if 2 == c_a.value and score3 > bomb_score:
                            score3 -= bomb_score#bomb possible to die
                            
                    else: # 1 == , None ==
                        score3 = score2
                        
                elif 1 == pity:                   
                    score3 = score2 - 8
                else: # None == pity
                    score3 = score2
                if score3 > max_p_score: #for turn color = player
                    max_p_score = score3
                    ch_player = ch_position3
                    pm_player = pm_p
                
                # unmarked 20190805
                if score3 > alpha:
                    break
                            
            #############################
            if max_p_score != -9000:
                                    
                if alpha > max_p_score:
                    alpha = max_p_score
                    
                m4.append([ch_position2, pm_com, ch_player, pm_player, max_p_score])
                
                ##########temp
                #if (0, 6) == nexti and (0, 7) == nextj:
                #    print('m4', m4)
                ##########end temp
                
                # unmarked 20190805
                if max_p_score < beta:
                    max_p_score = -9000
                    break                                        
                
                max_p_score = -9000
                    
            else:
                m4.append([ch_position2, pm_com, None, None, score2])
                #max_p_score = -9000
                
                ##########temp
                #if (0, 6) == nexti and (0, 7) == nextj:
                #    print('m4', m4)
                ##########end temp
            
            #print('m4', m4)
        ###############################
        if m4:
            min_index = m4.index(min(m4, key=lambda s:s[4]))
            coms = m4[min_index][4]
            ch_comp = m4[min_index][0]
            pm_comp = m4[min_index][1]
            
            alpha = AI_min_score
            
            if beta < coms:
                beta = coms
            
            m3.append([ch_position, pm, ch_comp, pm_comp, coms])
            
            m4 = []
            
            # marked 20191104
            #if coms > final_score:                            
            #    break

        else:
            m3.append([ch_position, pm, None, None,score])
            #m4 = []
        
        ##########temp
        #if (0, 6) == nexti and (0, 7) == nextj:
        #    print('m3', m3)
        ##########end temp
        
        #print('m3',m3)
        ###############################
    ###############################
    if m3:
        #print('m3=', m3)
        max_index = m3.index(max(m3, key=lambda s:s[4]))
        ps = m3[max_index][4]
        ch_1 = m3[max_index][0]
        pm_1 = m3[max_index][1]
        m2.append([mm[0], mm[1], ch_1, pm_1, ps])
    else:
        m2.append([mm[0], mm[1], None, None, sc])
        
    #print('m2', m2)
    ###############################
    
    gb_m2[ind] = m2
    
    q.put(gb_m2[ind])
    #return m2, af_map, af_ch

# dest_will_be_dead ...
cdef int dest_will_dead_owner_wont_eat(org, dest, a_ch, a_map, int opp_color):
    n = a_map[org[0]][org[1]]
    m = a_map[dest[0]][dest[1]]
    if None == n:
        return 0
    elif m != None:
    #eat
        return 0
    
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    af_map, af_ch = move(org, dest, af_map, af_ch)
    all_chess_move(af_map, af_ch)
    mm = af_map[dest[0]][dest[1]]
    my = af_ch[mm[0]][mm[1]]
    
    for chr in af_ch:
        for ch in chr:
            if ch == my:
                continue
            if 1 == ch.live and ch.back < 1 and ch.color == opp_color: 
                #for pm in ch.possible_move:
                #    if pm == dest:
                #        return 1
                if dest in ch.possible_move:
                    return 1
    return 0

# will_be_dead    
cdef int will_dead((int, int)org, a_ch, a_map, int opp_color):
    n = a_map[org[0]][org[1]]
    if None == n:
        return 0
    my = a_ch[n[0]][n[1]]
    for chr in a_ch:
        for ch in chr:
            if ch == my:
                continue
            if 1 == ch.live and ch.back < 1 and ch.color == opp_color: 
                #for pm in ch.possible_move:
                #    if pm == org:
                #        return 1
                if org in ch.possible_move:
                    return 1
    return 0

cdef int will_eat2_more(nexti, nextj, a_ch, a_map, int owner_color):    
    cdef int opp_color = 1-owner_color
    cdef int can_eat = 0
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    if nexti != None and nextj != None:
        af_map, af_ch = move(nexti, nextj, af_map, af_ch)
        all_chess_move(af_map, af_ch)
    for chr in af_ch:
        for ch in chr:
            if 1 == ch.live and ch.back < 1 and ch.color == owner_color:
                for pm in ch.possible_move:
                    n = af_map[pm[0]][pm[1]]
                    if n != None:
                        nch = af_ch[n[0]][n[1]]
                        if ch.value == nch.value:
                            continue
                    if 1 == stand_will_dead_pity(pm, af_ch, af_map, opp_color):
                        can_eat += 1
    if can_eat >= 2:
        return 1
    else:
        return 0

# return 0: NO way to escape 
# else return the more negative score the more possible to escape
cdef double escape_way_to_run(nexti, nextj, a_ch, a_map, int owner_color):
    opp_color = 1-owner_color
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    if nexti != None and nextj != None:
        af_map, af_ch = move(nexti, nextj, af_map, af_ch)
        all_chess_move(af_map, af_ch)
    m = af_map[nextj[0]][nextj[1]]
    my = af_ch[m[0]][m[1]]
    
    escape_step = 0
    for eat_pm in my.possible_move:
        if af_map[eat_pm[0]][eat_pm[1]] != None:
            eat_step = 0
            n = af_map[eat_pm[0]][eat_pm[1]]
            nch = af_ch[n[0]][n[1]]

            if nch.value == my.value:
                continue
            if 1 == nch.live and nch.back < 1 and nch.color == opp_color:
                if 1 == stand_will_dead_pity(eat_pm, af_ch, af_map, opp_color):
                    for pm in nch.possible_move:
                        if 1 == will_dead_pity_uncheck_will_dead(eat_pm, pm, af_ch, af_map, opp_color):
                            eat_step += 1
                    if eat_step == len(nch.possible_move):
                        return 0 # 0 greater than negative number
                    elif escape_step > eat_step - len(nch.possible_move):
                        escape_step = eat_step - len(nch.possible_move) #negative
                        
    if 0 == escape_step:
        return -0.09
    else:
        return escape_step/100

# stand_will_be_dead_pity    
cdef int stand_will_dead_pity((int , int)org, a_ch, a_map, int owner_color):
    cdef int opp_color = 1-owner_color
    n = a_map[org[0]][org[1]]
    if None == n:
        return 0
    my = a_ch[n[0]][n[1]]
    for chr in a_ch:
        for ch in chr:
            if ch == my:
                continue
            if 1 == ch.live and ch.back < 1 and ch.color == opp_color: 
                #for pm in ch.possible_move:
                #    if pm == org:
                #        if 0 == will_dead_pity((ch.row, ch.col) ,pm, a_ch, a_map, opp_color):
                #            return 1
                if org in ch.possible_move:
                    if 0 == will_dead_pity((ch.row, ch.col) , org, a_ch, a_map, opp_color):
                        return 1
    return 0

# will_be_dead_pity...
cdef int will_dead_pity_uncheck_will_dead(nexti, nextj, a_ch, a_map, int owner_color):
    global king_live
    
    (y, x) = nexti
    a = a_map[y][x]
    
    if nextj != None:
        (ii, jj) = nextj
        b = a_map[ii][jj]
        if b != None:
            if 2 == a_ch[a[0]][a[1]].value:
                if a_ch[b[0]][b[1]].value > 5:
                    return 0
            if a_ch[b[0]][b[1]].value == a_ch[a[0]][a[1]].value:
                return 0
    
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    af_map, af_ch = move(nexti, nextj, af_map, af_ch)
    all_chess_move(af_map, af_ch)
    opp_color = 1 - owner_color

    pity = 0
    i2 = None
    j2 = None
    i3 = None
    j3 = None
    
    for chr in af_ch:
        for ch in chr:
            if 1 == ch.back or 0 == ch.live:
                continue
            if ch.color == opp_color:
                if nextj in ch.possible_move:
                    if b == None:
                        i2 = (ch.row, ch.col)
                        j2 = nextj
                        pity = 1
                        
                        af2_map = copy.deepcopy(af_map)
                        af2_ch = copy.deepcopy(af_ch)
                        af2_map, af2_ch = move(i2, j2, af2_map, af2_ch)
                        all_chess_move(af2_map, af2_ch)
                        
                        (ii, jj) = j2
                        bb = af2_map[ii][jj]
                        
                        for chr in af2_ch:
                            for ch in chr:
                                if 1 == ch.back or 0 == ch.live:
                                    continue
                                if ch.color == owner_color:
                                    if j2 in ch.possible_move:
                                        if eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) <= eating_value_to_score(af2_ch[bb[0]][bb[1]].value, king_live, owner_color):
                                            i3 = (ch.row, ch.col)
                                            j3 = j2
                                            pity = 0
                                            
                                            af3_map = copy.deepcopy(af2_map)
                                            af3_ch = copy.deepcopy(af2_ch)
                                            af3_map, af3_ch = move(i3, j3, af3_map, af3_ch)
                                            all_chess_move(af3_map, af3_ch)
                                            
                                            for chr in af3_ch:
                                                for ch in chr:
                                                    if 1 == ch.back or 0 == ch.live:
                                                        continue
                                                    if ch.color == opp_color:
                                                        if j3 in ch.possible_move:
                                                            return 1
                                                            
                        if 1 == pity: return 1
                                            
                    elif eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) > eating_value_to_score(a_ch[b[0]][b[1]].value, king_live, owner_color):
                        i2 = (ch.row, ch.col)
                        j2 = nextj
                        pity = 1
                        
                        af2_map = copy.deepcopy(af_map)
                        af2_ch = copy.deepcopy(af_ch)
                        af2_map, af2_ch = move(i2, j2, af2_map, af2_ch)
                        all_chess_move(af2_map, af2_ch)
                        
                        (ii, jj) = j2
                        bbb = af2_map[ii][jj]
                        
                        for chr in af2_ch:
                            for ch in chr:
                                if 1 == ch.back or 0 == ch.live:
                                    continue
                                if ch.color == owner_color:
                                    if j2 in ch.possible_move:
                                        if eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) <= eating_value_to_score(af2_ch[bbb[0]][bbb[1]].value, king_live, owner_color):
                                            i3 = (ch.row, ch.col)
                                            j3 = j2
                                            pity = 0
                                            
                                            af3_map = copy.deepcopy(af2_map)
                                            af3_ch = copy.deepcopy(af2_ch)
                                            af3_map, af3_ch = move(i3, j3, af3_map, af3_ch)
                                            all_chess_move(af3_map, af3_ch)
                                            
                                            for chr in af3_ch:
                                                for ch in chr:
                                                    if 1 == ch.back or 0 == ch.live:
                                                        continue
                                                    if ch.color == opp_color:
                                                        if j3 in ch.possible_move:
                                                            return 1
                        if 1 == pity: return 1
                        
    return pity

# will_be_dead_pity...
def will_dead_pity_even_equal(nexti, nextj, a_ch, a_map, int owner_color):
    global king_live
    
    if None == nexti or None == nextj:
        return None
        
    #if 1 == will_dead(nexti, a_ch, a_map, 1-owner_color):
    #    return 0
    
    (y, x) = nexti
    a = a_map[y][x]
    
    if nextj != None:
        (ii, jj) = nextj
        b = a_map[ii][jj]
        if b != None:
            if 2 == a_ch[a[0]][a[1]].value:
                if a_ch[b[0]][b[1]].value > 5:
                    return 0
            if a_ch[b[0]][b[1]].value == a_ch[a[0]][a[1]].value:
                return 0
    
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    af_map, af_ch = move(nexti, nextj, af_map, af_ch)
    all_chess_move(af_map, af_ch)
    opp_color = 1 - owner_color

    pity = 0
    i2 = None
    j2 = None
    i3 = None
    j3 = None
    
    for chr in af_ch:
        for ch in chr:
            if 1 == ch.back or 0 == ch.live:
                continue
            if ch.color == opp_color:
                if nextj in ch.possible_move:
                    if b == None:
                        i2 = (ch.row, ch.col)
                        j2 = nextj
                        pity = 1
                        
                        af2_map = copy.deepcopy(af_map)
                        af2_ch = copy.deepcopy(af_ch)
                        af2_map, af2_ch = move(i2, j2, af2_map, af2_ch)
                        all_chess_move(af2_map, af2_ch)
                        
                        (ii, jj) = j2
                        bb = af2_map[ii][jj]
                        
                        for chr in af2_ch:
                            for ch in chr:
                                if 1 == ch.back or 0 == ch.live:
                                    continue
                                if ch.color == owner_color:
                                    if j2 in ch.possible_move:
                                        if eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) < eating_value_to_score(af2_ch[bb[0]][bb[1]].value, king_live, owner_color):
                                            i3 = (ch.row, ch.col)
                                            j3 = j2
                                            pity = 0
                                            
                                            af3_map = copy.deepcopy(af2_map)
                                            af3_ch = copy.deepcopy(af2_ch)
                                            af3_map, af3_ch = move(i3, j3, af3_map, af3_ch)
                                            all_chess_move(af3_map, af3_ch)
                                            
                                            for chr in af3_ch:
                                                for ch in chr:
                                                    if 1 == ch.back or 0 == ch.live:
                                                        continue
                                                    if ch.color == opp_color:
                                                        if j3 in ch.possible_move:
                                                            return 1
                                                            
                        if 1 == pity: return 1
                                            
                    elif eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) >= eating_value_to_score(a_ch[b[0]][b[1]].value, king_live, owner_color):
                        i2 = (ch.row, ch.col)
                        j2 = nextj
                        pity = 1
                        
                        af2_map = copy.deepcopy(af_map)
                        af2_ch = copy.deepcopy(af_ch)
                        af2_map, af2_ch = move(i2, j2, af2_map, af2_ch)
                        all_chess_move(af2_map, af2_ch)
                        
                        (ii, jj) = j2
                        bbb = af2_map[ii][jj]
                        
                        for chr in af2_ch:
                            for ch in chr:
                                if 1 == ch.back or 0 == ch.live:
                                    continue
                                if ch.color == owner_color:
                                    if j2 in ch.possible_move:
                                        if eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) < eating_value_to_score(af2_ch[bbb[0]][bbb[1]].value, king_live, owner_color):
                                            i3 = (ch.row, ch.col)
                                            j3 = j2
                                            pity = 0
                                            
                                            af3_map = copy.deepcopy(af2_map)
                                            af3_ch = copy.deepcopy(af2_ch)
                                            af3_map, af3_ch = move(i3, j3, af3_map, af3_ch)
                                            all_chess_move(af3_map, af3_ch)
                                            
                                            for chr in af3_ch:
                                                for ch in chr:
                                                    if 1 == ch.back or 0 == ch.live:
                                                        continue
                                                    if ch.color == opp_color:
                                                        if j3 in ch.possible_move:
                                                            return 1
                        if 1 == pity: return 1
                        
    return pity

# will_be_dead_pity
# Some situations are NOT considered    
def will_dead_pity(nexti, nextj, a_ch, a_map, int owner_color):
    global king_live
    
    if None == nexti or None == nextj:
        return None
    
    if 1 == will_dead(nexti, a_ch, a_map, 1-owner_color):
        return 0
    
    return will_dead_pity_uncheck_will_dead(nexti, nextj, a_ch, a_map, owner_color)
        
cdef int eating_value_to_score(int value, king, int owner_color):
    if 1 == value:
        if 1 == king[owner_color]:
            return 30
        else:
            return 25
    elif 2 == value:
        return 290
    elif 3 == value:
        return 28
    elif 4 == value:
        return 75
    elif 5 == value:
        return 165
    elif 6 == value:
        return 595
    elif 7 == value:
        return 1205

cdef void display_font(screen, int AI_vs_AI = 0):
    
    if 1 == player_win:        
        if 0 == AI_vs_AI:
            winer = u"玩家勝!!!"
        else:
            winer = u"電腦勝!!!"
        screen.blit(write(winer, (0, 0, 255)), (text_x, text_y))
    elif -1 == player_win:
        winer = u"電腦勝..."
        screen.blit(write(winer, (0, 255, 0)), (text_x, text_y))
    elif -2 == player_win:
        winer = u"強制結束..."
        screen.blit(write(winer, (0, 255, 0)), (text_x, text_y))
    elif 1 == first:
        if 1 == player_first:
            up = u"玩家先"
            screen.blit(write(up, (150, 150, 150)), (text_x, text_y))
        else:
            uc = u"電腦先"
            screen.blit(write(uc, (150, 150, 150)), (text_x, text_y))
    elif turn_id == player_color:
        up = u"玩家下"
        if 1 == player_color:
            screen.blit(write(up, (255, 0, 0)), (text_x, text_y))
        else:
            screen.blit(write(up, (0, 0, 0)), (text_x, text_y))
    else: #turn_id == com_color or turn_id == 2
        uc = u"電腦下"
        if 1 == com_color:
            screen.blit(write(uc, (255, 0, 0)), (text_x, text_y))
        else:
            screen.blit(write(uc, (0, 0, 0)), (text_x, text_y))       
        
def write(msg="pygame is cool", (int, int, int)color= (0,0,0)):    
    #myfont = pygame.font.SysFont("None", 32) #To avoid py2exe error
    myfont = pygame.font.Font("wqy-zenhei.ttf",14)
    mytext = myfont.render(msg, True, color)
    mytext = mytext.convert_alpha()
    return mytext 

def clean_back_n1_to_0(a_ch):
    for chr in a_ch:
        for ch in chr:
            if -1 == ch.back:
                ch.back = 0
    return a_ch

# AI_vs_AI -> 0: human vs AI, 1: AI vs AI    
def main(int AI_vs_AI = 0, int AI_Limit_step = 200):
    global cstart_x
    global cstart_y
    global cstart_x2
    global cstart_y2
    global chess_index
    global turn_id
    global player_color
    global com_color
    global player_first
    global server_main_chess
    global main_chess
    global main_map
    global cor
    global first
    global player_win
    global chess_num
    global com_moving_end
    global back_num
    global king_live
    global move_step
    global sindex
    global break_long_capture_dest
    global break_long_capture_org
    global com_ban_step
    global back_value_num
    global step
    
    pygame.display.set_icon(pygame.image.load("Image/darkchess_default.png"))
    screen = pygame.display.set_mode(SCREEN_SIZE, 0, 32)#SCREEN_SIZE, FULLSCREEN, 32)
    pygame.display.set_caption("Taiwan Blind Chess")
    
    chess_back = pygame.image.load(image_chess_back).convert_alpha()

    chess_bk = pygame.image.load(image_chess_bk).convert_alpha()
    chess_ba = pygame.image.load(image_chess_ba).convert_alpha()
    chess_bb = pygame.image.load(image_chess_bb).convert_alpha()
    chess_br = pygame.image.load(image_chess_br).convert_alpha()
    chess_bn = pygame.image.load(image_chess_bn).convert_alpha()
    chess_bc = pygame.image.load(image_chess_bc).convert_alpha()
    chess_bp = pygame.image.load(image_chess_bp).convert_alpha()
    chess_rk = pygame.image.load(image_chess_rk).convert_alpha()
    chess_ra = pygame.image.load(image_chess_ra).convert_alpha()
    chess_rb = pygame.image.load(image_chess_rb).convert_alpha()
    chess_rr = pygame.image.load(image_chess_rr).convert_alpha()
    chess_rn = pygame.image.load(image_chess_rn).convert_alpha()
    chess_rc = pygame.image.load(image_chess_rc).convert_alpha()
    chess_rp = pygame.image.load(image_chess_rp).convert_alpha()
    
    chess_bks = pygame.image.load(image_chess_bks).convert_alpha()
    chess_bas = pygame.image.load(image_chess_bas).convert_alpha()
    chess_bbs = pygame.image.load(image_chess_bbs).convert_alpha()
    chess_brs = pygame.image.load(image_chess_brs).convert_alpha()
    chess_bns = pygame.image.load(image_chess_bns).convert_alpha()
    chess_bcs = pygame.image.load(image_chess_bcs).convert_alpha()
    chess_bps = pygame.image.load(image_chess_bps).convert_alpha()
    chess_rks = pygame.image.load(image_chess_rks).convert_alpha()
    chess_ras = pygame.image.load(image_chess_ras).convert_alpha()
    chess_rbs = pygame.image.load(image_chess_rbs).convert_alpha()
    chess_rrs = pygame.image.load(image_chess_rrs).convert_alpha()
    chess_rns = pygame.image.load(image_chess_rns).convert_alpha()
    chess_rcs = pygame.image.load(image_chess_rcs).convert_alpha()
    chess_rps = pygame.image.load(image_chess_rps).convert_alpha()
    
    chess_image_sel = [chess_bps, chess_bcs, chess_bns, chess_brs, chess_bbs, chess_bas, chess_bks, chess_rps, chess_rcs, chess_rns, chess_rrs, chess_rbs, chess_ras, chess_rks]
    chess_image = [chess_bp, chess_bc, chess_bn, chess_br, chess_bb, chess_ba, chess_bk, chess_rp, chess_rc, chess_rn, chess_rr, chess_rb, chess_ra, chess_rk, chess_back]
    
    background = pygame.image.load(background_image_filename).convert_alpha()
    new_game   = pygame.image.load(image_new).convert_alpha()

    while True:
        selected_c = None
        player_win = 0
        turn_id = 0
        player_color = 0
        com_color = 1
        first = 1
        moving = 1
        com_mv = 0
        com_moving_end = 1
        game_start = 1
        chess_num[:] = [16, 16]
        back_num = 32
        break_long_capture_dest = []
        break_long_capture_org = []
        com_ban_step = []
        move_step = [None, None, None, None]
        sindex = 0
        step = 0
        back_value_num[0][:] = [0, 5, 2, 2, 2, 2, 2, 1]
        back_value_num[1][:] = [0, 5, 2, 2, 2, 2, 2, 1]
        #back_value_num = [[0, 5, 2, 2, 2, 2, 2, 1], [0, 5, 2, 2, 2, 2, 2, 1]]
        
        player_first = random.randint(0, 1)
        
        if 1 == AI_vs_AI:
            player_first = 0
        
        chess_index = ini_random_chess(chess_index)
        for i in range(0, 4):
            for j in range(0, 4):
                ch = chess(chess_index[8*i+j], (i, j))
                server_main_chess[i][j] = ch
                # 32 is invalid value for initiation value only
                main_chess[i][j] = chess(32, (i,j))
                cor[i][j] = (ch.x, ch.y)
                main_map[i][j] = (i, j)
        for i in range(0, 4):
            for j in range(0, 4):
                ch = chess(chess_index[8*i+4+j], (i, 4+j))
                server_main_chess[i][4+j] = ch
                # 32 is invalid value for initiation value only
                main_chess[i][4+j] = chess(32, (i, 4+j))
                cor[i][4+j] = (ch.x, ch.y)
                main_map[i][4+j] = (i, 4+j)
        
		# Test data
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 28
        #
        #chess_num[0] = 15
        #chess_num[1] = 16
        #
        #main_chess[1][1].live = 0
        #main_map[1][1] = None
        #
        #ch = chess(5, (1, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][0] = ch
        #main_map[1][0] = (1, 0)
        #
        #ch = chess(13, (2, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][0] = ch
        #main_map[2][0] = (2, 0)
        #
        #ch = chess(21, (0, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][0] = ch
        #main_map[0][0] = (0, 0)
        #
        #ch = chess(25, (1, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][2] = ch
        #main_map[1][2] = (1, 2)

        #End Test data
        
        # Test data 2
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #
        #chess_num[0] = 2
        #chess_num[1] = 2
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(15, (1, 3))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][3] = ch
        #main_map[1][3] = (1, 3)
        #
        #ch = chess(13, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        #
        #ch = chess(29, (2, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][4] = ch
        #main_map[2][4] = (2, 4)
        #
        #ch = chess(27, (1, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][7] = ch
        #main_map[1][7] = (1, 7)
        
        #End Test data 2
        
        # Test data 3
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #
        #chess_num[0] = 3
        #chess_num[1] = 4
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(14, (1, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][1] = ch
        #main_map[1][1] = (1, 1)
        #
        #ch = chess(13, (3, 3))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][3] = ch
        #main_map[3][3] = (3, 3)
        #
        #ch = chess(29, (2, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][4] = ch
        #main_map[2][4] = (2, 4) 
        #
        #ch = chess(27, (0, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][5] = ch
        #main_map[0][5] = (0, 5) 
        
        #End Test data 3
        
        # Test data 4
        #first = 0
        #com_color = 1
        #player_color = 0
        #turn_id = 1
        #back_num = 0
        #king_live[0] = 0 
        #
        #chess_num[0] = 4
        #chess_num[1] = 3
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #        
        #ch = chess(11, (0, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][0] = ch
        #main_map[0][0] = (0, 0)
        #
        #ch = chess(9, (1, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][0] = ch
        #main_map[1][0] = (1, 0)
        #
        #ch = chess(0, (2, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][0] = ch
        #main_map[2][0] = (2, 0)
        #
        #ch = chess(25, (0, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][1] = ch
        #main_map[0][1] = (0, 1)
        #
        #ch = chess(27, (1, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][1] = ch
        #main_map[1][1] = (1, 1)
        #
        #ch = chess(13, (2, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][1] = ch
        #main_map[2][1] = (2, 1)
        #
        #ch = chess(31, (2, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][4] = ch
        #main_map[2][4] = (2, 4)
        
        # End Test data 4
        
        # Test data 5
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #chess_num[0] = 2
        #chess_num[1] = 2
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(13, (1, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][1] = ch
        #main_map[1][1] = (1, 1)
        #
        #ch = chess(15, (0, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][1] = ch
        #main_map[0][1] = (0, 1)
        #
        #ch = chess(29, (3, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][0] = ch
        #main_map[3][0] = (3, 0)
        #
        #ch = chess(23, (1, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][7] = ch
        #main_map[1][7] = (1, 7)
        
        # End test data 5
        
        # Test data 6
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #king_live[1] = 0
        #
        #chess_num[0] = 4
        #chess_num[1] = 4
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(15, (3, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][6] = ch
        #main_map[3][6] = (3, 6)
        #
        #ch = chess(9, (3, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][7] = ch
        #main_map[3][7] = (3, 7)
        #
        #ch = chess(16, (3, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][5] = ch
        #main_map[3][5] = (3, 5)
        #
        #ch = chess(27, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        #
        #ch = chess(13, (1, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][6] = ch
        #main_map[1][6] = (1, 6)
        #
        #ch = chess(21, (2, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][0] = ch
        #main_map[2][0] = (2, 0)
        #
        #ch = chess(22, (2, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][2] = ch
        #main_map[2][2] = (2, 2)
        
        # End test data 6
        
        # test data 7
        
        #first = 0
        #com_color = 1
        #player_color = 0
        #turn_id = 1
        #back_num = 0
        #king_live[0] = 0
        #king_live[1] = 0
        #
        #chess_num[0] = 4
        #chess_num[1] = 2
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(13, (2, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][2] = ch
        #main_map[2][2] = (2, 2)
        #
        #ch = chess(5, (3, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][7] = ch
        #main_map[3][7] = (3, 7)
        #
        #ch = chess(0, (2, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][7] = ch
        #main_map[2][7] = (2, 7)
        #
        #ch = chess(1, (3, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][6] = ch
        #main_map[3][6] = (3, 6)
        #
        #ch = chess(29, (3, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][1] = ch
        #main_map[3][1] = (3, 1)
        #
        #ch = chess(27, (2, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][6] = ch
        #main_map[2][6] = (2, 6)
        
        # End test data 7
        
        # test data 8
        
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #chess_num[0] = 2
        #chess_num[1] = 2
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(13, (1, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][4] = ch
        #main_map[1][4] = (1, 4)
        #
        #ch = chess(15, (0, 3))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][3] = ch
        #main_map[0][3] = (0, 3)
        #
        #ch = chess(29, (1, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][2] = ch
        #main_map[1][2] = (1, 2)
        #
        #ch = chess(23, (1, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][7] = ch
        #main_map[1][7] = (1, 7)
        
        # End test data 8
        
        # Test data 9
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #king_live[1] = 0
        #
        #chess_num[0] = 4
        #chess_num[1] = 4
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(15, (3, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][6] = ch
        #main_map[3][6] = (3, 6)
        #
        #ch = chess(9, (3, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][7] = ch
        #main_map[3][7] = (3, 7)
        #
        #ch = chess(16, (2, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][5] = ch
        #main_map[2][5] = (2, 5)
        #
        #ch = chess(27, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        #
        #ch = chess(13, (1, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][7] = ch
        #main_map[1][7] = (1, 7)
        #
        #ch = chess(21, (2, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][0] = ch
        #main_map[2][0] = (2, 0)
        #
        #ch = chess(0, (0, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][1] = ch
        #main_map[0][1] = (0, 1)
        #
        #ch = chess(17, (0, 3))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][3] = ch
        #main_map[0][3] = (0, 3)
        
        # End test data 9
        
        # Test data 10
        #first = 0
        #com_color = 1
        #player_color = 0
        #turn_id = 1
        #back_num = 0
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #        
        #ch = chess(15, (1, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][6] = ch
        #main_map[1][6] = (1, 6)
        #
        #ch = chess(13, (0, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][6] = ch
        #main_map[0][6] = (0, 6)
        #
        #ch = chess(11, (1, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][5] = ch
        #main_map[1][5] = (1, 5)
        #
        #ch = chess(9, (0, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][5] = ch
        #main_map[0][5] = (0, 5)
        #
        #ch = chess(31, (0, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][7] = ch
        #main_map[0][7] = (0, 7)
        #
        #ch = chess(29, (1, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][4] = ch
        #main_map[1][4] = (1, 4)
        
        # End test data 10
        
        # Test data 11
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 0
        #king_live[1] = 0
        #
        #chess_num[0] = 4
        #chess_num[1] = 4
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(15, (3, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][6] = ch
        #main_map[3][6] = (3, 6)
        #
        #ch = chess(9, (3, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][7] = ch
        #main_map[3][7] = (3, 7)
        #
        #ch = chess(16, (2, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][5] = ch
        #main_map[2][5] = (2, 5)
        #
        #ch = chess(21, (2, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][0] = ch
        #main_map[2][0] = (2, 0)
        #
        #ch = chess(13, (0, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][1] = ch
        #main_map[0][1] = (0, 1)
        #
        #ch = chess(29, (1, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][2] = ch
        #main_map[1][2] = (1, 2)
        #
        #ch = chess(27, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        
        # End test data 11
        
        # Test data 12
        #first = 0
        #com_color = 1
        #player_color = 0
        #turn_id = 1
        #back_num = 0
        #king_live[1] = 0
        #
        #chess_num[0] = 8
        #chess_num[1] = 2
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(13, (1, 0))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][0] = ch
        #main_map[1][0] = (1, 0)
        #
        #ch = chess(14, (1, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][2] = ch
        #main_map[1][2] = (1, 2)
        #
        #ch = chess(9, (2, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][2] = ch
        #main_map[2][2] = (2, 2)
        #
        #ch = chess(15, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        #
        #ch = chess(10, (2, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][5] = ch
        #main_map[2][5] = (2, 5)
        #
        #ch = chess(11, (3, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][7] = ch
        #main_map[3][7] = (3, 7)
        #
        #ch = chess(12, (1, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][6] = ch
        #main_map[1][6] = (1, 6)
        #
        #ch = chess(27, (0, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][1] = ch
        #main_map[0][1] = (0, 1)
        #
        #ch = chess(28, (2, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][1] = ch
        #main_map[2][1] = (2, 1)
        
        # End test data 12
        
        # Test data 13
        #first = 0
        #com_color = 1
        #player_color = 0
        #turn_id = 1
        #back_num = 1
        #king_live[0] = 0
        #king_live[1] = 0
        #
        #chess_num[0] = 3
        #chess_num[1] = 2
        #
        #for c in [0, 1]:
        #    for v in range(1, 8):
        #        back_value_num[c][v] = 0
        #
        #back_value_num[0][5] = 1
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        if 1 == i and 6 == j:
        #            continue
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(11, (1, 6))
        #ch.back = 1
        #ch.live = 1
        #server_main_chess[1][6] = ch
        #main_map[1][6] = (1, 6)
        #
        #ch = chess(21, (1, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][5] = ch
        #main_map[1][5] = (1, 5)
        #
        #ch = chess(16, (1, 1))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][1] = ch
        #main_map[1][1] = (1, 1)
        #
        #ch = chess(9, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        #
        #ch = chess(10, (0, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][7] = ch
        #main_map[0][7] = (0, 7)
        
        # End test data 13
        
        # Test data 14
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 3
        #
        #chess_num[0] = 9
        #chess_num[1] = 6
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        if 2 == i and 3 == j:
        #            continue
        #        if 0 == i and 5 == j:
        #            continue
        #        if 2 == i and 7 == j:
        #            continue
        #        main_chess[i][j].live = 0
        #        main_map[i][j] = None
        #
        #ch = chess(13, (1, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][5] = ch
        #main_map[1][5] = (1, 5)
        #
        #ch = chess(7, (1, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][6] = ch
        #main_map[1][6] = (1, 6)
        #
        #ch = chess(8, (2, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][6] = ch
        #main_map[2][6] = (2, 6)
        #
        #ch = chess(23, (3, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][5] = ch
        #main_map[3][5] = (3, 5)
        #
        #ch = chess(24, (1, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][4] = ch
        #main_map[1][4] = (1, 4)
        #
        #ch = chess(29, (2, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][4] = ch
        #main_map[2][4] = (2, 4)
        #
        #ch = chess(31, (3, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][7] = ch
        #main_map[3][7] = (3, 7)
        #
        #ch = chess(0, (0, 6))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][6] = ch
        #main_map[0][6] = (0, 6)
        #
        #ch = chess(1, (3, 2))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][2] = ch
        #main_map[3][2] = (3, 2)
        #
        #ch = chess(11, (0, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[0][7] = ch
        #main_map[0][7] = (0, 7)
        #
        #ch = chess(15, (1, 7))
        #ch.back = 0
        #ch.live = 1
        #main_chess[1][7] = ch
        #main_map[1][7] = (1, 7)
        #
        #ch = chess(14, (3, 3))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][3] = ch
        #main_map[3][3] = (3, 3)
        
        # End test data 14
        
        # Test data 15
        #first = 0
        #com_color = 0
        #player_color = 1
        #turn_id = 0
        #back_num = 26
        #
        #for i in range(0, 4):
        #    for j in range(0, 8):
        #        if 3 == i and j > 3:
        #            main_chess[i][j].live = 0
        #            main_map[i][j] = None
        #        elif 2 == i and 6 > j > 3:
        #            main_chess[i][j].live = 0
        #            main_map[i][j] = None
        #
        #ch = chess(7, (2, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[2][5] = ch
        #main_map[2][5] = (2, 5)
        #
        #ch = chess(9, (3, 4))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][4] = ch
        #main_map[3][4] = (3, 4)
        #
        #ch = chess(29, (3, 5))
        #ch.back = 0
        #ch.live = 1
        #main_chess[3][5] = ch
        #main_map[3][5] = (3, 5)
        
        # End test data 15
        
        while 0 == player_win:
            if 1 == game_start:
                sound_new = pygame.mixer.Sound(s_newgame)
                sound_new.play()
                game_start = 0

            screen.blit(background, (0,0))
            screen.blit(new_game, (new_game_iconi, new_game_iconj))
            
            display_font(screen, AI_vs_AI)
            for cr in main_chess:
                for c in cr:
                    c.draw(screen, chess_image_sel, chess_image)
                       
            no_move = 1
            if 2 == turn_id:
                for cr in main_chess:
                    for c in cr:
                        com_mv = 0
                        if c.x != cor[c.row][c.col][0]:
                            c.x = c.x+1 if c.x < cor[c.row][c.col][0] else c.x-1
                            com_mv = 1
                            if (c.x, c.y) == cor[c.row][c.col]:                                
                                if main_map[c.row][c.col] != None:
                                    (desti, destj) = main_map[c.row][c.col]
                                    dest_ch = main_chess[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                main_map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if c.y != cor[c.row][c.col][1]:
                            c.y = c.y+1 if c.y < cor[c.row][c.col][1] else c.y-1
                            com_mv = 1  
                            if (c.x, c.y) == cor[c.row][c.col]:                                
                                if main_map[c.row][c.col] != None:
                                    (desti, destj) = main_map[c.row][c.col]
                                    dest_ch = main_chess[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                main_map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if 1 == com_mv:
                            no_move = 0
                            c.draw(screen, chess_image_sel, chess_image)
                            com_mv = 0
                if 1 == no_move:
                    turn_id = player_color
            
            if selected_c != None:
                selected_c.move()
                selected_c.draw(screen, chess_image_sel, chess_image)
            
            chess_ai()
            
            if 1 == AI_vs_AI and turn_id != 2:
                player_color, com_color = com_color, player_color
                if AI_Limit_step == step:
                    player_win = -2
                
                for event in pygame.event.get():
                    if event.type == QUIT:
                        exit()
            
            if 0 == back_num and 1 == cant_move(main_map, main_chess, player_color):
                    print('player cant move')
                    player_win = -1
            
            for event in pygame.event.get():
                if event.type == QUIT:
                    exit()
                elif turn_id == player_color:
                    if event.type == pygame.MOUSEBUTTONDOWN and turn_id == player_color:
                        (mouseX, mouseY) = pygame.mouse.get_pos()
                        if new_game_iconi < mouseX < new_game_iconi + new_game.get_width() and new_game_iconj < mouseY < new_game_iconj + new_game.get_height():
                            player_win = -1
                        all_chess_move(main_map, main_chess)
                        sound_click = pygame.mixer.Sound(s_click)
                        sound_click.play()
                        click_once = 0
                        for i, chr in enumerate(main_chess):
                            for j, chc in enumerate(chr):
                                if 0 == click_once:
                                    ch_index = chc.click((mouseX, mouseY))
                                    if ch_index != None:
                                        if 1 == player_first and 1 == first:
                                            cindex = color_value_to_index(server_main_chess[i][j].color, server_main_chess[i][j].value, back_value_num)
                                            # ch_index may different with cindex
                                            main_chess[i][j] = chess(cindex, (i,j))
                                            main_chess[i][j].back = 0
                                            turn_id = index_to_color(cindex)
                                            player_color = turn_id
                                            com_color = 1 - player_color
                                            first = 0
                                            selected_c = None
                                            back_num -= 1
                                            turn_id = com_color
                                            back_value_num[player_color][index_to_chess_value(cindex)] -= 1
                                            step += 1
                                            #print(back_value_num)
                                        elif -1 == ch_index and chc.color == player_color:
                                            selected_c = chc
                                        elif ch_index != -1 and 0 == first:
                                            cindex = color_value_to_index(server_main_chess[i][j].color, server_main_chess[i][j].value, back_value_num)
                                            main_chess[i][j] = chess(cindex, (i, j))
                                            main_chess[i][j].back = 0
                                            selected_c = None
                                            back_num -= 1
                                            back_value_num[index_to_color(cindex)][index_to_chess_value(cindex)] -= 1
                                            turn_id = com_color
                                            step += 1
                                            #print(back_value_num)
                                        click_once = 1
                                        break
                    elif event.type == pygame.MOUSEBUTTONUP and turn_id == player_color:
                        if selected_c != None:
                            (mouseX, mouseY) = pygame.mouse.get_pos()
                            moving = 0
                            for pm in selected_c.possible_move:
                                if pm == mouse_position_to_block(mouseX, mouseY, chess_back):
                                    if main_map[pm[0]][pm[1]] != None:
                                        main_chess[main_map[pm[0]][pm[1]][0]][main_map[pm[0]][pm[1]][1]].live = 0
                                        chess_num[main_chess[main_map[pm[0]][pm[1]][0]][main_map[pm[0]][pm[1]][1]].color] -= 1
                                        sound_capture = pygame.mixer.Sound(s_capture)
                                        sound_capture.play()
                                    else:
                                        sound_move = pygame.mixer.Sound(s_move2)
                                        sound_move.play()
                                    main_map[pm[0]][pm[1]] = main_map[selected_c.row][selected_c.col]
                                    main_map[selected_c.row][selected_c.col] = None
                                    org = (selected_c.row, selected_c.col)
                                    selected_c.x = cor[pm[0]][pm[1]][0]
                                    selected_c.y = cor[pm[0]][pm[1]][1]
                                    selected_c.row = pm[0]
                                    selected_c.col = pm[1]
                                    dest = (selected_c.row, selected_c.col)
                                    moving = 1
                                    turn_id = com_color
                                    step += 1
                                    
                                    possible_mv = collect_possible_move(selected_c.row, selected_c.col, main_map, main_chess)
                                    move_step[sindex] = [selected_c.color, org, dest, possible_mv]

                                    sindex = (sindex+1)%4
                                    
                                    br = 0
                                    while(br < len(break_long_capture_dest)):
                                        b = 0
                                        for d in break_long_capture_dest[br]:
                                            if dest == d:
                                                del break_long_capture_dest[br]
                                                del break_long_capture_org[br]
                                                del com_ban_step[br]
                                                b = 1
                                                break
                                        if 0 == b:
                                            br += 1

                                    br = 0
                                    while(br < len(break_long_capture_org)):
                                        b = 0
                                        for o in break_long_capture_org[br]:
                                            if org == o:
                                                del break_long_capture_dest[br]
                                                del break_long_capture_org[br]
                                                del com_ban_step[br]
                                                b = 1
                                                break
                                        if 0 == b:
                                            br += 1
                    
                                    break
                            
                            if 0 == moving:
                               (selected_c.x, selected_c.y) = cor[selected_c.row][selected_c.col] 
                        
                            selected_c.speed = 0
                            selected_c = None
                            moving = 1
                            break
                        else:
                            moving = 1
            
            if 1 == moving:
                all_chess_move(main_map, main_chess)
                moving = 0
            
            if selected_c != None:
                (mouseX, mouseY) = pygame.mouse.get_pos()
                dx = mouseX - selected_c.x
                dy = mouseY - selected_c.y
                dx -= selected_c.size[0]/2
                dy -= selected_c.size[1]/2
                selected_c.angle = 0.5*math.pi + math.atan2(dy, dx)
                selected_c.speed = math.hypot(dx, dy) * 0.1
            
            if 0 == chess_num[player_color]:
                #Open all chess
                for i, cr in enumerate(main_chess):
                    for j, c in enumerate(cr):
                        if 32 == c.index and 1 == main_chess[i][j].live:
                            cindex = color_value_to_index(server_main_chess[i][j].color, server_main_chess[i][j].value, back_value_num)
                            main_chess[i][j] = chess(cindex, (i, j))
                            main_chess[i][j].back = 0
                player_win = -1
                
            elif 0 == chess_num[com_color]:
                #Open all chess
                for i, cr in enumerate(main_chess):
                    for j, c in enumerate(cr):
                        if 32 == c.index and 1 == main_chess[i][j].live:
                            cindex = color_value_to_index(server_main_chess[i][j].color, server_main_chess[i][j].value, back_value_num)
                            main_chess[i][j] = chess(cindex, (i, j))
                            main_chess[i][j].back = 0
                player_win = 1
            
            if 1 == player_win:
                screen.blit(background, (0,0))
                display_font(screen, AI_vs_AI)
                for cr in main_chess:
                    for c in cr:
                        c.draw(screen, chess_image_sel, chess_image)
                sound_win = pygame.mixer.Sound(s_win)
                sound_win.play()
                pygame.display.update()
                time.sleep(5)
            elif -1 == player_win or -2 == player_win:
                screen.blit(background, (0,0))
                display_font(screen, AI_vs_AI)
                for cr in main_chess:
                    for c in cr:
                        c.draw(screen, chess_image_sel, chess_image)
                sound_loss = pygame.mixer.Sound(s_loss)
                sound_loss.play()
                pygame.display.update()
                time.sleep(5)
            
            pygame.display.update()
            
    exit()
