from alttester import AltDriver
from pages.helpermethods import HelperMethods
import pytest


class TestPlay:
    alt_driver = AltDriver()

    def setup_method(self):
        self.helper_methods = HelperMethods(self.alt_driver)


    @pytest.mark.parametrize("flow, msg",
                             [([1, 5, 9, 3, 7, 8, 4], "O Player Wins!"),
                              ([8, 5, 9, 7, 3, 6, 4, 1, 2], "It's a Draw! Try Again."),
                              ([9, 7, 5, 1, 6, 4], "X Player Wins!")])
    def test_flow_cases(self, flow, msg):
        self.helper_methods.restart_button.click()
        self.helper_methods.flow_slots_played(flow)
        self.helper_methods.assert_win_info(msg)
        self.alt_driver.get_png_screenshot(msg + ".png")