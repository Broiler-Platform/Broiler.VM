// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The program that wrote temporal-calendars.icu4x-2.3.0.txt: ICU4X's answer for the conversion lines
// of temporal-calendars.js, each CLDR calendar's fields of the same ISO dates, in the program's line
// format. It is a binary crate's src/main.rs whose Cargo.toml depends on
//
//     icu_calendar = { version = "=2.3.0", default-features = false, features = ["compiled_data"] }
//     calendrical_calculations = "=0.2.4"
//
// the two crates src/tests/calendars/pins archives; `cargo run --release > temporal-calendars.icu4x-2.3.0.txt`.
use icu_calendar::{AnyCalendar, AnyCalendarKind, Date, types::YearInfo};

fn days_from_civil(y: i64, m: i64, d: i64) -> i64 {
    let y = if m <= 2 { y - 1 } else { y };
    let era = if y >= 0 { y } else { y - 399 } / 400;
    let yoe = y - era * 400;
    let doy = (153 * (m + if m > 2 { -3 } else { 9 }) + 2) / 5 + d - 1;
    let doe = yoe * 365 + yoe / 4 - yoe / 100 + doy;
    era * 146097 + doe - 719468
}

fn civil_from_days(z: i64) -> (i64, i64, i64) {
    let z = z + 719468;
    let era = if z >= 0 { z } else { z - 146096 } / 146097;
    let doe = z - era * 146097;
    let yoe = (doe - doe / 1460 + doe / 36524 - doe / 146096) / 365;
    let y = yoe + era * 400;
    let doy = doe - (365 * yoe + yoe / 4 - yoe / 100);
    let mp = (5 * doy + 2) / 153;
    let d = doy - (153 * mp + 2) / 5 + 1;
    let m = if mp < 10 { mp + 3 } else { mp - 9 };
    (if m <= 2 { y + 1 } else { y }, m, d)
}

fn iso_string(y: i64, m: i64, d: i64) -> String {
    let year = if (0..=9999).contains(&y) { format!("{:04}", y) } else if y < 0 { format!("-{:06}", -y) } else { format!("+{:06}", y) };
    format!("{}-{:02}-{:02}", year, m, d)
}

fn main() {
    let calendars = [
        ("buddhist", AnyCalendarKind::Buddhist), ("chinese", AnyCalendarKind::Chinese), ("coptic", AnyCalendarKind::Coptic),
        ("dangi", AnyCalendarKind::Dangi), ("ethioaa", AnyCalendarKind::EthiopianAmeteAlem), ("ethiopic", AnyCalendarKind::Ethiopian),
        ("gregory", AnyCalendarKind::Gregorian), ("hebrew", AnyCalendarKind::Hebrew), ("indian", AnyCalendarKind::Indian),
        ("islamic-civil", AnyCalendarKind::HijriTabularTypeIIFriday), ("islamic-tbla", AnyCalendarKind::HijriTabularTypeIIThursday),
        ("islamic-umalqura", AnyCalendarKind::HijriUmmAlQura), ("japanese", AnyCalendarKind::Japanese), ("persian", AnyCalendarKind::Persian),
        ("roc", AnyCalendarKind::Roc),
    ];
    let mut dates = Vec::new();
    let mut day = days_from_civil(1850, 1, 1);
    while day <= days_from_civil(2050, 12, 31) { dates.push(civil_from_days(day)); day += 613; }
    for (y, m, d) in [(-500, 7, 1), (0, 1, 1), (1, 1, 1), (622, 7, 19), (1000, 6, 15), (1582, 10, 15), (1700, 3, 1),
                      (1872, 12, 31), (1873, 1, 1), (1912, 7, 30), (1926, 12, 25), (1989, 1, 8), (2019, 5, 1), (2101, 1, 29),
                      (2200, 2, 1), (3000, 9, 1), (10000, 1, 1)] { dates.push((y, m, d)); }
    for (name, kind) in calendars {
        
        for &(y, m, d) in &dates {
            let date = Date::from_rata_die(calendrical_calculations::rata_die::RataDie::new(days_from_civil(y, m, d) + 719163), AnyCalendar::new(kind));
            let (era, era_year) = match date.year() {
                YearInfo::Era(e) => (e.era.to_string(), e.year.to_string()),
                _ => (String::new(), String::new()),
            };
            let month = date.month();
            println!("{} {} => {},{},{},{},{},{},{},{},{},{},{}", name, iso_string(y, m, d), era, era_year,
                date.year().extended_year(), month.ordinal, month.to_input().code(), date.day_of_month().0,
                date.day_of_year().0, date.days_in_month(), date.days_in_year(), date.months_in_year(), date.is_in_leap_year());
        }
    }
}
