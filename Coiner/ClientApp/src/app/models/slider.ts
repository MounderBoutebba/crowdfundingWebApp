import { Constants } from '../constants';

export class Slider {
    sliderValue: number;
    sliderMin: number;
    sliderMax: number;
    disabledSlider: boolean;

    constructor() {
        // this.sliderValue = 0;
        this.sliderMin = 0;
        this.sliderMax = Constants.UserCoinsNumber;
        this.disabledSlider = false;
    }
}