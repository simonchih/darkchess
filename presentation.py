"""Display-only flip animation. Never changes a piece or calls the AI."""
import math
import pygame
from chess_data import index_to_chess_surface, index_to_chess_select


def flip_surface(back, face, progress):
    progress = max(0.0, min(1.0, progress))
    eased = (1 - math.cos(math.pi * progress)) / 2
    width = max(2, round(back.get_width() * abs(math.cos(math.pi * eased))))
    source = back if progress < 0.5 else face
    result = pygame.transform.smoothscale(source, (width, source.get_height()))
    if width < 5:
        result.fill((195, 150, 80, 255))
    return result


class FlipAnimator:
    def __init__(self):
        self.previous = {(r, c): True for r in range(4) for c in range(8)}

    def present(self, screen, board, selected, images, background, new_game, status):
        pieces = [p for row in board for p in row if p.live]
        reveals = [p for p in pieces if self.previous.get((p.row, p.col), False) and p.back != 1]
        self.previous = {(p.row, p.col): p.back == 1 for p in pieces}
        if not reveals:
            return
        # Rebuild the scene so the old front never flashes before the animation.
        screen.blit(background, (0, 0))
        screen.blit(new_game, (440, 13))
        status(screen)
        for p in pieces:
            p.draw(screen, selected, images)
        scene = screen.copy()
        for p in reveals:
            area = pygame.Rect(p.x, p.y, 57, 57)
            scene.blit(background, area, area)
        clock = pygame.time.Clock()
        start = pygame.time.get_ticks()
        while True:
            progress = min(1.0, (pygame.time.get_ticks() - start) / 420.0)
            for event in pygame.event.get([pygame.QUIT, pygame.MOUSEBUTTONDOWN, pygame.MOUSEBUTTONUP, pygame.MOUSEMOTION]):
                if event.type == pygame.QUIT:
                    pygame.quit()
                    raise SystemExit
            screen.blit(scene, (0, 0))
            for p in reveals:
                face = index_to_chess_select(p.index, selected) if p.back == -1 else index_to_chess_surface(p.index, images)
                frame = flip_surface(images[14], face, progress)
                screen.blit(frame, (p.x + (57-frame.get_width())//2, p.y))
            pygame.display.update()
            if progress >= 1:
                break
            clock.tick(60)
