import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faBan,
  faCheck,
  faEdit,
  faHome,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faStar,
  faUser,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faBan,
  faCheck,
  faEdit,
  faHome,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faStar,
  faUser,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
