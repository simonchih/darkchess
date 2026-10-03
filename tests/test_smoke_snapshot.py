import os
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

os.environ['PYGAME_HIDE_SUPPORT_PROMPT'] = '1'
import pygame
from smoke_test import save_snapshot


class Snapshot(unittest.TestCase):
    def test_snapshot_uses_new_file_and_preserves_existing_preview(self):
        with tempfile.TemporaryDirectory() as folder:
            previous = Path(folder) / 'game.png'
            previous.write_bytes(b'previous preview')
            report = {}
            save_snapshot(pygame.Surface((32, 32)), folder, report)
            self.assertNotIn('screenshot_warning', report)
            self.assertEqual(previous.read_bytes(), b'previous preview')
            self.assertEqual(pygame.image.load(str(Path(folder)/report['screenshot'])).get_size(), (32,32))

    def test_optional_screenshot_write_failure_is_reported(self):
        report = {}
        with patch.object(Path, 'write_bytes', side_effect=PermissionError('image file locked')):
            save_snapshot(pygame.Surface((32,32)), '.', report)
        self.assertIn('image file locked', report['screenshot_warning'])
        self.assertNotIn('screenshot', report)


if __name__ == '__main__':
    unittest.main()
