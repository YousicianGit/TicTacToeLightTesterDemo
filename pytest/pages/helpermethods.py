from alttester import By, AltObject


class HelperMethods:

    def __init__(self, alt_driver):
        self.alt_driver = alt_driver

    @property
    def restart_button(self) -> AltObject:
        return self.alt_driver.wait_for_object(By.TEXT, 'RESTART', timeout=3)

    def win_info(self):
        return self.alt_driver.wait_for_object(By.NAME, 'Text (Legacy)', timeout=3)

    def slot_number(self, number: int):
        return self.alt_driver.wait_for_object(By.NAME,
                                              f"Dice Button ({number - 1})")

    def flow_slots_played(self, slots):
        for slot in slots:
            place = self.slot_number(slot)
            assert place is not None
            place.click()

    def assert_win_info(self, msg_result):
        assert self.win_info() is not None
        assert self.win_info().get_text() == msg_result
